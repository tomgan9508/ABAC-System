using Common.Application.Clients;
using Common.Application.Dtos.Authorization;
using Common.Application.Models.API;
using Common.Cosmos.Models.Entities;
using Common.Cosmos.Models.Properties;
using Common.Cosmos.Providers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationManagement.ResourceManager
{
    public class AuthorizationManager : IAuthorizationManager
    {
        // api urls for local testing
        private const string AuthorizationEngineApiUrl = "http://localhost:5860/api/authorizationEngine";
        private const string PreAuthorizationEngineApiUrl = "http://localhost:7151/api/preAuthorizationEngine";

        private const int MaxPoliciesPerBatch = 20; // this is configurable after we have some performance tests
        private readonly IUsersContainerProvider _usersContainerProvider;
        private readonly IResourcesContainerProvider _resourcesContainerProvider;
        private readonly IHttpClientExtension _httpClientExtension;
        private readonly ILogger<AuthorizationManager> _logger;

        public AuthorizationManager(
            IUsersContainerProvider usersContainerProvider,
            IResourcesContainerProvider resourcesContainerProvider,
            IHttpClientExtension httpClientExtension,
            ILogger<AuthorizationManager> logger)
        {
            _usersContainerProvider = usersContainerProvider;
            _resourcesContainerProvider = resourcesContainerProvider;
            _httpClientExtension = httpClientExtension;
            _logger = logger;
        }

        public async Task<AuthorizationResponse> IsAuthorizedAsync(string userId, string resourceId)
        {
            var resource = (await _resourcesContainerProvider.GetResource(resourceId))?.Resource?.Properties;
            var user = (await _usersContainerProvider.GetUserInfoByPartition<AbacUserEntity>(userId))?
                .Resource.FirstOrDefault()?.Properties;
            var userApprovals = (await _usersContainerProvider.GetUserInfoByPartition<UserApprovedPoliciesEntity>(userId))?
                .Resource.FirstOrDefault()?.Properties;

            if (resource is null || user is null)
            {
                return null;
            }

            // if resource doesn't have any policy, it means that it is public
            if (resource.Properties.Policies is null || !resource.Properties.Policies.Any())
            {
                return new AuthorizationResponse(true);
            }

            // if user has already approved a policy for this resource,
            // we don't need to check again
            if (userApprovals is not null && userApprovals.ApprovedPolicies.Any() &&
                await IsUserPreAuthorized(resource.Properties.Policies, userApprovals.ApprovedPolicies, user))
            {
                return new AuthorizationResponse(true);
            }

            // if user has not approved any policy for this resource,
            // we need to check using the authorization engine
            Task<AuthorizationEngineResponse[]> tasks = Task.WhenAll(resource.Properties.Policies.Chunk(MaxPoliciesPerBatch)
                .Select(chunk => CreateAuthorizationRequest(chunk, user.Properties.Attributes)));

            try
            {
                var result = await tasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error ocurred when trying to calculate policies authorization. " +
                                     $"UserId: {userId}, resourceId: {resourceId}. Error: {ex.Message}");

                throw;
            }

            string fulfilledPolicyId = tasks.Result.FirstOrDefault(r => r.ContainsFulfilledPolicy)?.PolicyFulfilledId;
            bool isUserAuthorized = !string.IsNullOrEmpty(fulfilledPolicyId);
            
            if (isUserAuthorized)
            {
                await _usersContainerProvider.AddUserApprovedPolicy(userId, fulfilledPolicyId);
            }

            return new AuthorizationResponse(isUserAuthorized);
        }

        private async Task<bool> IsUserPreAuthorized(
            IEnumerable<string> policyIds, 
            IDictionary<string, DateTime> userApprovedPolicie,
            AbacUserProperties user)
        {
            PreAuthorizationEngineResponse response = await CreatePreAuthorizationRequest(policyIds, user.LastUpdated, userApprovedPolicie);

            if (response.ExpiredPolicyApprovals.Any())
            {
                await RemoveExpiredPolicies(response.ExpiredPolicyApprovals, userApprovedPolicie, user.Properties.Id);
            }

            return response.UnExpiredPolicyApprovals.Any();
        }

        // remove expired policies from user approved policies
        private async Task<UserApprovedPoliciesEntity> RemoveExpiredPolicies(
            IEnumerable<string> expiredPolicyApprovals, 
            IDictionary<string, DateTime> userApprovedPolicie,
            string userId)
        {
            foreach (string expiredPolicy in expiredPolicyApprovals)
            {
                userApprovedPolicie.Remove(expiredPolicy);
            }

            return await _usersContainerProvider.CreateOrUpdateUserApprovedPolicies(userId, userApprovedPolicie);
        }

        // creates authorization request to the authorization engine
        // to check if user is authorized to access this resource
        private Task<AuthorizationEngineResponse> CreateAuthorizationRequest(IEnumerable<string> policyIds, IDictionary<string, object> attributes)
        {
            Uri uri = new(AuthorizationEngineApiUrl);
            string content = JsonConvert.SerializeObject(new AuthorizationEngineRequest(policyIds, attributes));

            return _httpClientExtension.PostAsync<AuthorizationEngineResponse>(uri, content);
        }

        // creates a pre authorization request to the pre authorization engine
        // to check if user has already approved any policy for this resource
        private Task<PreAuthorizationEngineResponse> CreatePreAuthorizationRequest
            (IEnumerable<string> policyIds, 
            DateTime userLastUpdated,
            IDictionary<string, DateTime> userApprovedPolicies)
        {
            Uri uri = new(PreAuthorizationEngineApiUrl);
            string content = JsonConvert.SerializeObject(new PreAuthorizationEngineRequest(policyIds, userLastUpdated, userApprovedPolicies));

            return _httpClientExtension.PostAsync<PreAuthorizationEngineResponse>(uri, content);
        }
    }
}
