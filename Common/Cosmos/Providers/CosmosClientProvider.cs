using Microsoft.Azure.Cosmos;

namespace Common.Cosmos.Providers
{
    public class CosmosClientProvider : ICosmosClientProvider
    {
        public CosmosClient CreateClient()
        {
            var clientOptions = new CosmosClientOptions
            {
                AllowBulkExecution = true,
                MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(60),
                MaxRetryAttemptsOnRateLimitedRequests = 19
            };

            return new CosmosClient(
                    accountEndpoint: Constants.Constants.LocalAccountEndpoint,
                    authKeyOrResourceToken: Constants.Constants.LocalAuthKeyOrResourceToken,
                    clientOptions);
        }
    }
}
