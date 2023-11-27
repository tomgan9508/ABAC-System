using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Common.Cosmos.Providers;
using System.Collections.Generic;
using System.Linq;
using Common.Application.Dtos.Authorization;
using Common.Cosmos.Models.Properties;
using AuthorizationEngine.Engines;

namespace AuthorizationEngine.Functions
{
    public class AuthorizationEngine
    {
        private readonly IPoliciesContainerProvider _policiesContainerProvider;
        private readonly IAttributesContainerProvider _attributesContainerProvider;
        private readonly IPoliciesCheckerEngine _policiesCheckerEngine;
        private readonly ILogger<AuthorizationEngine> _logger;

        public AuthorizationEngine(
            IPoliciesContainerProvider policiesContainerProvider,
            IPoliciesCheckerEngine policiesCheckerEngine,
            IAttributesContainerProvider attributesContainerProvider,
            ILogger<AuthorizationEngine> logger)
        {
            _policiesContainerProvider = policiesContainerProvider;
            _policiesCheckerEngine = policiesCheckerEngine;
            _attributesContainerProvider = attributesContainerProvider;
            _logger = logger;
        }

        [FunctionName(nameof(AuthorizationEngineFunction))]
        public async Task<IActionResult> AuthorizationEngineFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "authorizationEngine")] 
            HttpRequest req)
        {
            _logger.LogInformation($"Starting {nameof(AuthorizationEngineFunction)} Function...");

            var authorizationEngineDto = JsonConvert.DeserializeObject<AuthorizationEngineRequest>(await req.ReadAsStringAsync());

            if (authorizationEngineDto is null)
            {
                return new BadRequestObjectResult("Invalid request body");
            }

            List<PolicyProperties> policies = (await _policiesContainerProvider.GetPolicies(authorizationEngineDto.ResourcePolicyIds))?.Resource
                .Select(entity => entity.Properties)
                .Where(policy => policy != null)
                .ToList();

            IDictionary<string, string> attributeTypes = (await _attributesContainerProvider.GetAttributes(authorizationEngineDto.Attributes.Keys))?.Resource
                .Select(entity => entity.Properties)
                .Where(attribute => attribute != null)
                .ToDictionary(_ => _.Properties.Name, _ => _.Properties.Type);

            return new OkObjectResult(await _policiesCheckerEngine.Execute(policies, authorizationEngineDto.Attributes, attributeTypes));
        }
    }
}
