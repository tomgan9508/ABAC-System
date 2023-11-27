using AttributesManagement.ResourceManager;
using Common.Application.Models.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace AttributesManagement.ResourceProviders
{
    public class AttributeProvider
    {
        private readonly IAttributeManager _attributeManager;
        private readonly ILogger<AttributeProvider> _logger;

        public AttributeProvider(IAttributeManager attributeManager, ILogger<AttributeProvider> logger)
        {
            _attributeManager = attributeManager;
            _logger = logger;
        }

        [FunctionName(nameof(GetAttribute))]
        public async Task<IActionResult> GetAttribute(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "attributes/{attributeName}")]
            HttpRequest req,
            string attributeName)
        {
            _logger.LogInformation($"Starting {nameof(GetAttribute)} Function...");

            if (string.IsNullOrEmpty(attributeName))
            {
                return new BadRequestObjectResult("Missing attributeName");
            };

            UserAttribute attribute = (await _attributeManager.GetAttribute(attributeName))?.Properties?.Properties;

            if (attribute is null)
            {
                return new NotFoundObjectResult("Attribute not found");
            }

            return new OkObjectResult(attribute);
        }

        [FunctionName(nameof(CreateAttribute))]
        public async Task<IActionResult> CreateAttribute(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "attributes")]
            HttpRequest req)
        {
            _logger.LogInformation($"Starting {nameof(CreateAttribute)} Function...");

            var requestBody = JsonConvert.DeserializeObject<UserAttribute>(await req.ReadAsStringAsync());

            if (requestBody is null)
            {
                return new BadRequestObjectResult("Invalid request body");
            }

            UserAttribute attribute = (await _attributeManager.CreateAttribute(requestBody))?.Properties?.Properties;

            return new OkObjectResult(attribute);
        }
    }
}
