using Common.Application.Models.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ResourceManagement.ResourceManager;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResourceManagement.ResourceProviders
{
    public class AbacResourceProvider
    {
        private readonly IAbacResourceManager _resourceManager;
        private readonly ILogger<AbacResourceProvider> _logger;

        public AbacResourceProvider(IAbacResourceManager resourceManager, ILogger<AbacResourceProvider> logger)
        {
            _resourceManager = resourceManager;
            _logger = logger;
        }

        [FunctionName(nameof(GetResource))]
        public async Task<IActionResult> GetResource(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "resources/{resourceId}")]
            HttpRequest req,
            string resourceId )
        {
            _logger.LogInformation($"Starting {nameof(GetResource)} Function...");

            if (string.IsNullOrEmpty(resourceId))
            {
                return new BadRequestObjectResult("Missing resourceId");
            };

            AbacResource resource = (await _resourceManager.GetResource(resourceId))?.Properties?.Properties;

            if (resource is null)
            {
                return new NotFoundObjectResult($"Resource {resourceId} not found");
            }

            return new OkObjectResult(resource);
        }

        [FunctionName(nameof(CreateResource))]
        public async Task<IActionResult> CreateResource(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "resources")]
            HttpRequest req)
        {
            _logger.LogInformation($"Starting {nameof(CreateResource)} Function...");

            var requestBody = JsonConvert.DeserializeObject<AbacResource>(await req.ReadAsStringAsync());

            if (requestBody is null)
            {
                return new BadRequestObjectResult("Invalid request body");
            }

            AbacResource resource = (await _resourceManager.CreateResource(requestBody))?.Properties?.Properties;

            return new OkObjectResult(resource);
        }

        [FunctionName(nameof(UpdateResourcePolicies))]
        public async Task<IActionResult> UpdateResourcePolicies(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "resources/{resourceId}")]
            HttpRequest req,
            string resourceId)
        {
            _logger.LogInformation($"Starting {nameof(UpdateResourcePolicies)} Function...");

            if (string.IsNullOrEmpty(resourceId))
            {
                return new BadRequestObjectResult("Missing resourceId.");
            };

            var newPolicies = JsonConvert.DeserializeObject<KeyValuePair<string, IEnumerable<string>>>(await req.ReadAsStringAsync());
            AbacResource resource = (await _resourceManager.UpdateResourcePolicies(resourceId, newPolicies.Value))?.Properties?.Properties;

            if (resource is null)
            {
                return new NotFoundObjectResult($"Resource {resourceId} not found.");
            }

            return new OkObjectResult(resource);
        }
    }
}
