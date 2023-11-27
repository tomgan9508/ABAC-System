using PreAuthorization;
using Common.Cosmos.Local;
using Common.Cosmos.Providers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using PreAuthorization.Engines;

[assembly: FunctionsStartup(typeof(Startup))]

namespace PreAuthorization
{
    public class Startup : FunctionsStartup
    {
        public Startup()
        {
            LocalCosmosContainerInit.CreateLocalDatabaseAndContainers();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IPoliciesContainerProvider, PoliciesContainerProvider>();
            builder.Services.AddSingleton<IPreAuthorizationEngine, PreAuthorizationEngine>();
            builder.Services.AddSingleton<ICosmosClientProvider, CosmosClientProvider>();
            builder.Services.AddSingleton((serviceProvider) => serviceProvider.GetService<ICosmosClientProvider>().CreateClient());
        }
    }
}
