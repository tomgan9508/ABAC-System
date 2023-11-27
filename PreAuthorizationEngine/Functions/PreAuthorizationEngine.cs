using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Common.Cosmos.Models.Properties;
using Common.Application.Dtos.Authorization;
using Common.Cosmos.Providers;
using PreAuthorization.Engines;

namespace PreAuthorization.Functions
{
    public class PreAuthorizationEngine
    {
        private readonly IPoliciesContainerProvider _policiesContainerProvider;
        private readonly IPreAuthorizationEngine _preAuthorizationEngine;
        private readonly ILogger<PreAuthorizationEngine> _logger;

        public PreAuthorizationEngine(
            IPoliciesContainerProvider policiesContainerProvider,
            IPreAuthorizationEngine preAuthorizationEngine,
            ILogger<PreAuthorizationEngine> logger)
        {
            _policiesContainerProvider = policiesContainerProvider;
            _preAuthorizationEngine = preAuthorizationEngine;
            _logger = logger;
        }

        [FunctionName(nameof(PreAuthorizationEngineFunction))]
        public async Task<IActionResult> PreAuthorizationEngineFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "preAuthorizationEngine")] 
            HttpRequest req)
        {
            _logger.LogInformation($"Starting {nameof(PreAuthorizationEngineFunction)} Function...");

            var preAuthorizationEngineDto = JsonConvert.DeserializeObject<PreAuthorizationEngineRequest>(await req.ReadAsStringAsync());

            if (preAuthorizationEngineDto is null)
            {
                return new BadRequestObjectResult("Invalid request body");
            }

            List<PolicyProperties> policies = (await _policiesContainerProvider.GetPolicies(preAuthorizationEngineDto.ResourcePolicyIds))?.Resource
                .Select(entity => entity.Properties)
                .Where(policy => policy != null)
                .ToList();

            return new OkObjectResult(await _preAuthorizationEngine.AnalyzeUserPreAuthorization(
                policies, preAuthorizationEngineDto.UserLastUpdated, preAuthorizationEngineDto.UserApprovedPolicies));
        }
    }
}
