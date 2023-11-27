using AuthorizationManagement;
using AuthorizationManagement.ResourceManager;
using Common.Application.Clients;
using Common.Cosmos.Local;
using Common.Cosmos.Providers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(Startup))]

namespace AuthorizationManagement
{
    public class Startup : FunctionsStartup
    {
        public Startup()
        {
            LocalCosmosContainerInit.CreateLocalDatabaseAndContainers();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient(string.Empty).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler() { MaxConnectionsPerServer = 500 });

            builder.Services.AddSingleton<IUsersContainerProvider, UsersContainerProvider>();
            builder.Services.AddSingleton<IAuthorizationManager, AuthorizationManager>();
            builder.Services.AddSingleton<IResourcesContainerProvider, ResourcesContainerProvider>();
            builder.Services.AddSingleton<IHttpClientExtension, HttpClientExtension>();
            builder.Services.AddSingleton<ICosmosClientProvider, CosmosClientProvider>();
            builder.Services.AddSingleton((serviceProvider) => serviceProvider.GetService<ICosmosClientProvider>().CreateClient());
        }
    }
}