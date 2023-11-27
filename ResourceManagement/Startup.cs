using ResourceManagement;
using Common.Cosmos.Local;
using Common.Cosmos.Providers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ResourceManagement.ResourceManager;

[assembly: FunctionsStartup(typeof(Startup))]

namespace ResourceManagement
{
    public class Startup : FunctionsStartup
    {
        public Startup()
        {
            LocalCosmosContainerInit.CreateLocalDatabaseAndContainers();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IAbacResourceManager, AbacResourceManager>();
            builder.Services.AddSingleton<IResourcesContainerProvider, ResourcesContainerProvider>();
            builder.Services.AddSingleton<ICosmosClientProvider, CosmosClientProvider>();
            builder.Services.AddSingleton((serviceProvider) => serviceProvider.GetService<ICosmosClientProvider>().CreateClient());
        }
    }
}