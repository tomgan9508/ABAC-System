using Microsoft.Azure.Cosmos;

namespace Common.Cosmos.Providers
{
    public interface ICosmosClientProvider
    {
        CosmosClient CreateClient();
    }
}
