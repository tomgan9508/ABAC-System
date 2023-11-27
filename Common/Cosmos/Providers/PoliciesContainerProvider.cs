using Common.Cosmos.Models.Entities;
using Common.Cosmos.Models.Properties;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Common.Cosmos.Providers
{
    public class PoliciesContainerProvider : ContainerProviderBase, IPoliciesContainerProvider
    {
        public PoliciesContainerProvider(ILogger<UsersContainerProvider> logger, CosmosClient cosmosClient)
            : base(cosmosClient, Constants.Constants.CosmosDatabaseName, Constants.Constants.PoliciesContainerName, logger)
        {
        }

        public Task<ItemResponse<PolicyEntity>> GetPolicy(string policyId)
        {
            string entityId = PolicyEntity.GenerateId(policyId);
            string entitypk = PolicyEntity.GeneratePartitionKey(policyId);

            return Container.GetItem<PolicyEntity>(entityId, entitypk);
        }

        public Task<FeedResponse<PolicyEntity>> GetPolicies(IEnumerable<string> policyIds)
        {
            List<(string, string)> items = policyIds.Select(policyId =>
                {
                    string id = PolicyEntity.GenerateId(policyId);
                    string pk = PolicyEntity.GeneratePartitionKey(policyId);

                    return (id, pk);
                })
                .ToList();

            return Container.GetItems<PolicyEntity>(items);
        }

        public Task<ItemResponse<PolicyEntity>> CreatePolicy(PolicyProperties policy, bool returnResponse = false)
        {
            return Container.UpsertItem(new PolicyEntity(policy), returnResponse);
        }

        public Task<ItemResponse<PolicyEntity>> UpdatePolicy(PolicyProperties policy, bool returnResponse = false)
        {
            policy.UpdateLastUpdated();
            return Container.UpdateItem(new PolicyEntity(policy), returnResponse);
        }
    }
}
