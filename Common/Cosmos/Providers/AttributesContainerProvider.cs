using Common.Cosmos.Models.Entities;
using Common.Cosmos.Models.Properties;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Common.Cosmos.Providers
{
    public class AttributesContainerProvider : ContainerProviderBase, IAttributesContainerProvider
    {
        public AttributesContainerProvider(ILogger<UsersContainerProvider> logger, CosmosClient cosmosClient)
            : base(cosmosClient, Constants.Constants.CosmosDatabaseName, Constants.Constants.AttributesContainerName, logger)
        {
        }

        public Task<ItemResponse<UserAttributeEntity>> GetAttribute(string attributeName)
        {
            string entityId = UserAttributeEntity.GenerateId(attributeName);
            string entitypk = UserAttributeEntity.GeneratePartitionKey(attributeName);

            return Container.GetItem<UserAttributeEntity>(entityId, entitypk);
        }

        public Task<FeedResponse<UserAttributeEntity>> GetAttributes(IEnumerable<string> attributeNames)
        {
            List<(string, string)> items = attributeNames.Select(attributeName =>
            {
                string id = UserAttributeEntity.GenerateId(attributeName);
                string pk = UserAttributeEntity.GeneratePartitionKey(attributeName);

                return (id, pk);
            })
            .ToList();

            return Container.GetItems<UserAttributeEntity>(items);
        }

        public Task<ItemResponse<UserAttributeEntity>> CreateAttribute(UserAttributeProperties attribute, bool returnResponse = false)
        {
            return Container.UpsertItem(new UserAttributeEntity(attribute), returnResponse);
        }
    }
}
