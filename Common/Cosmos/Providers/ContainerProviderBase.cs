using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Common.Cosmos.Providers
{
    public abstract class ContainerProviderBase
    {
        protected ICosmosContainer Container { get; set; }

        public ContainerProviderBase(CosmosClient cosmosClient, string databaseName, string containerName, ILogger<ContainerProviderBase> logger)
        {
            Container = new CosmosContainer(cosmosClient.GetContainer(databaseName, containerName), logger);
        }
    }
}
