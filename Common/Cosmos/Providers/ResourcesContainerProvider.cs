using Common.Cosmos.Models.Entities;
using Common.Cosmos.Models.Properties;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Common.Cosmos.Providers
{
    public class ResourcesContainerProvider : ContainerProviderBase, IResourcesContainerProvider
    {
        public ResourcesContainerProvider(ILogger<ResourcesContainerProvider> logger, CosmosClient cosmosClient)
            : base(cosmosClient, Constants.Constants.CosmosDatabaseName, Constants.Constants.ResourcesContainerName, logger)
        {
        }

        public Task<ItemResponse<AbacResourceEntity>> GetResource(string resourceId)
        {
            string entityId = AbacResourceEntity.GenerateId(resourceId);
            string entitypk = AbacResourceEntity.GeneratePartitionKey(resourceId);

            return Container.GetItem<AbacResourceEntity>(entityId, entitypk);
        }

        public Task<ItemResponse<AbacResourceEntity>> CreateResource(AbacResourceProperties resource, bool returnResponse = false)
        {
            return Container.UpsertItem(new AbacResourceEntity(resource), returnResponse);
        }

        public Task<ItemResponse<AbacResourceEntity>> UpdateResource(AbacResourceProperties resource, bool returnResponse = false)
        {
            resource.UpdateLastUpdated();
            return Container.UpdateItem(new AbacResourceEntity(resource), returnResponse);
        }
    }
}
