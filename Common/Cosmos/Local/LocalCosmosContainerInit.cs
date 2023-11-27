using Microsoft.Azure.Cosmos;
using System.Diagnostics.CodeAnalysis;

namespace Common.Cosmos.Local
{
    /// <summary>
    /// This class is used to initialize the local Cosmos DB emulator with the required database and containers.
    /// </summary>
    public static class LocalCosmosContainerInit
    {
        [SuppressMessage("Usage", "VSTHRD002:Avoid problematic synchronous waits", Justification = "Synchronous callback")]
        [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP013:Await in using", Justification = "Synchronous callback")]
        public static void CreateLocalDatabaseAndContainers()
        {
            try
            {
                using CosmosClient cosmosClient = new(
                    accountEndpoint: Constants.Constants.LocalAccountEndpoint,
                    authKeyOrResourceToken: Constants.Constants.LocalAuthKeyOrResourceToken,
                    new CosmosClientOptions() { AllowBulkExecution = true });

                DatabaseResponse databaseResponse = cosmosClient.CreateDatabaseIfNotExistsAsync(Constants.Constants.CosmosDatabaseName).GetAwaiter().GetResult();

                IEnumerable<Task<ContainerResponse>> tasks = Containers.Select(container =>
                    databaseResponse.Database.CreateContainerIfNotExistsAsync(new ContainerProperties(container, $"/{Constants.Constants.PartitionKeyPropertyName}")));

                Task.WhenAll(tasks).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to initialize local Cosmos DB emulator: " + ex.Message);
            }
        }

        public static string[] Containers { get; } = new string[]
        {
             Constants.Constants.UsersContainerName,
             Constants.Constants.AttributesContainerName,
             Constants.Constants.PoliciesContainerName,
             Constants.Constants.ResourcesContainerName
        };
    }
}
