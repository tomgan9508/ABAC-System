using PolicyManagement;
using Common.Cosmos.Local;
using Common.Cosmos.Providers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using PolicyManagement.ResourceManager;

[assembly: FunctionsStartup(typeof(Startup))]

namespace PolicyManagement
{
    public class Startup : FunctionsStartup
    {
        public Startup()
        {
            LocalCosmosContainerInit.CreateLocalDatabaseAndContainers();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IPolicyManager, PolicyManager>();
            builder.Services.AddSingleton<IPoliciesContainerProvider, PoliciesContainerProvider>();
            builder.Services.AddSingleton<ICosmosClientProvider, CosmosClientProvider>();
            builder.Services.AddSingleton((serviceProvider) => serviceProvider.GetService<ICosmosClientProvider>().CreateClient());
        }
    }
}