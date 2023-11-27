using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Common.Application.Models.API;
using System.Collections.Generic;
using AuthorizationManagement.ResourceManager;

namespace AuthorizationManagement.ResourceProviders
{
    public class AuthorizationProvider
    {
        private readonly IAuthorizationManager _authorizationManager;
        private readonly ILogger<AuthorizationProvider> _logger;

        public AuthorizationProvider(IAuthorizationManager authorizationManager, ILogger<AuthorizationProvider> logger)
        {
            _authorizationManager = authorizationManager;
            _logger = logger;
        }

        [FunctionName(nameof(IsAuthorized))]
        public async Task<IActionResult> IsAuthorized(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "isAuthorized")]
            HttpRequest req)
        {
            _logger.LogInformation($"Starting {nameof(IsAuthorized)} Function...");

            if (!TryParseQueryParams(req, out string userId, out string resourceId))
            {
                return new BadRequestObjectResult("Missing userId or resourceId");
            }

            AuthorizationResponse response = await _authorizationManager.IsAuthorizedAsync(userId, resourceId);

            if (response is null)
            {
                return new NotFoundObjectResult($"The user {userId} or the resource {resourceId} was not found in our system.");
            }

            return new OkObjectResult(response);
        }

        private static bool TryParseQueryParams(HttpRequest request, out string userId, out string resourceId)
        {
            IDictionary<string, string> queryParams = request.GetQueryParameterDictionary();
            userId = queryParams.ContainsKey("userId") ? queryParams["userId"] : null;
            resourceId = queryParams.ContainsKey("resourceId") ? queryParams["resourceId"] : null;

            return !string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(resourceId);
        }
    }
}
