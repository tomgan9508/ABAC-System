using Common.Application;
using Common.Cosmos.Models.Entities;
using Common.Cosmos.Models.Properties;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;

namespace Common.Cosmos.Providers
{
    public class UsersContainerProvider : ContainerProviderBase, IUsersContainerProvider
    {
        public UsersContainerProvider(ILogger<UsersContainerProvider> logger, CosmosClient cosmosClient)
            : base(cosmosClient, Constants.Constants.CosmosDatabaseName, Constants.Constants.UsersContainerName, logger)
        {
        }

        public Task<ItemResponse<AbacUserEntity>> GetUser(string userId)
        {
            string entityId = AbacUserEntity.GenerateId(userId);
            string entitypk = AbacUserEntity.GeneratePartitionKey(userId);

            return Container.GetItem<AbacUserEntity>(entityId, entitypk);
        }

        public Task<ItemResponse<AbacUserEntity>> CreateUser(AbacUserProperties user, bool returnResponse = false)
        {
            return Container.UpsertItem(new AbacUserEntity(user), returnResponse);
        }

        public Task<ItemResponse<AbacUserEntity>> UpdateUser(AbacUserProperties user, bool returnResponse = false)
        {
            user.UpdateLastUpdated();
            return Container.UpdateItem(new AbacUserEntity(user), returnResponse);
        }

        public Task<FeedResponse<T>> GetUserInfoByPartition<T>(string userId) where  T : CosmosEntity
        {
            string partitionKey = AbacUserEntity.GeneratePartitionKey(userId);
            string query = Container.GetItemLinqQueryable<CosmosEntity>()
                .Where(entity => entity.PartitionKey == partitionKey)
                .Where(entity => entity.Type == typeof(T).GetField("EntityType").GetValue(typeof(T)))
                .ToQueryDefinition()
                .QueryText;

            return Container.ExecuteQuery<T>(query, partitionKey);
        }

        public Task<ItemResponse<UserApprovedPoliciesEntity>> CreateOrUpdateUserApprovedPolicies(string userId, IDictionary<string, DateTime> approvedPolicies, bool returnResponse = false)
        {
            var userApprovedPolicies = new UserApprovedPoliciesProperties(userId, approvedPolicies, SystemIdGenerator.GenerateId(userId));
            return Container.UpsertItem(new UserApprovedPoliciesEntity(userApprovedPolicies), returnResponse);
        }

        public Task<ItemResponse<UserApprovedPoliciesEntity>> GetUserApprovedPolicies(string userId)
        {
            string entityId = UserApprovedPoliciesEntity.GenerateId(userId);
            string entitypk = UserApprovedPoliciesEntity.GeneratePartitionKey(userId);

            return Container.GetItem<UserApprovedPoliciesEntity>(entityId, entitypk);
        }
        public async Task<ItemResponse<UserApprovedPoliciesEntity>> AddUserApprovedPolicy(string userId, string policyId)
        {
            IDictionary<string, DateTime> approvedPolicies = new Dictionary<string, DateTime>();
            var userAprovedPolicies =  (await GetUserApprovedPolicies(userId))?.Resource;
            if (userAprovedPolicies is not null)
            {
                approvedPolicies = userAprovedPolicies.Properties.ApprovedPolicies;
            }

            if (!approvedPolicies.ContainsKey(policyId))
            {
                approvedPolicies.Add(policyId, DateTime.UtcNow);
            }

            return await CreateOrUpdateUserApprovedPolicies(userId, approvedPolicies);
        }

    }
}
