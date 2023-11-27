using Common.Application.Models.API;
using Common.Cosmos.Models.Properties;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PolicyManagement.ResourceManager;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PolicyManagement.ResourceProviders
{
    public class PolicyProvider
    {
        private readonly IPolicyManager _policyManager;
        private readonly ILogger<PolicyProvider> _logger;

        public PolicyProvider(IPolicyManager policyManager, ILogger<PolicyProvider> logger)
        {
            _policyManager = policyManager;
            _logger = logger;
        }

        [FunctionName(nameof(GetPolicy))]
        public async Task<IActionResult> GetPolicy(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "policies/{policyId}")]
            HttpRequest req,
            string policyId)
        {
            _logger.LogInformation($"Starting {nameof(GetPolicy)} Function...");

            if (string.IsNullOrEmpty(policyId))
            {
                return new BadRequestObjectResult("Missing policyId");
            };

            Policy policy = (await _policyManager.GetPolicy(policyId))?.Properties?.Properties;

            if (policy is null)
            {
                return new NotFoundObjectResult($"Policy {policyId} not found");
            }

            return new OkObjectResult(policy);
        }

        [FunctionName(nameof(CreatePolicy))]
        public async Task<IActionResult> CreatePolicy(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "policies")]
            HttpRequest req)
        {
            _logger.LogInformation($"Starting {nameof(CreatePolicy)} Function...");

            var requestBody = JsonConvert.DeserializeObject<Policy>(await req.ReadAsStringAsync());

            if (requestBody is null)
            {
                return new BadRequestObjectResult("Invalid request body");
            }

            Policy policy = (await _policyManager.CreatePolicy(requestBody))?.Properties?.Properties;

            return new OkObjectResult(policy);
        }

        [FunctionName(nameof(UpdatePolicy))]
        public async Task<IActionResult> UpdatePolicy(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "policies/{policyId}")]
            HttpRequest req,
             string policyId)
        {
            _logger.LogInformation($"Starting {nameof(UpdatePolicy)} Function...");

            if (string.IsNullOrEmpty(policyId))
            {
                return new BadRequestObjectResult("Missing policyId.");
            };

            var newConditions = JsonConvert.DeserializeObject<KeyValuePair<string, IEnumerable<PolicyCondition>>>(await req.ReadAsStringAsync());
            Policy policy = (await _policyManager.UpdatePolicyConditions(policyId, newConditions.Value))?.Properties?.Properties;

            if (policy is null)
            {
                return new NotFoundObjectResult($"Policy {policyId} not found.");
            }

            return new OkObjectResult(policy);
        }
    }
}
