using AttributesManagement;
using AttributesManagement.ResourceManager;
using Common.Cosmos.Local;
using Common.Cosmos.Providers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace AttributesManagement
{
    public class Startup : FunctionsStartup
    {
        public Startup()
        {
            LocalCosmosContainerInit.CreateLocalDatabaseAndContainers();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IAttributeManager, AttributeManager>();
            builder.Services.AddSingleton<IAttributesContainerProvider, AttributesContainerProvider>();
            builder.Services.AddSingleton<ICosmosClientProvider, CosmosClientProvider>();
            builder.Services.AddSingleton((serviceProvider) => serviceProvider.GetService<ICosmosClientProvider>().CreateClient());
        }
    }
}