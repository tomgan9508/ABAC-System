using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using Common.Application.Models.API;
using Microsoft.Extensions.Logging;
using UserManagement.ResourceManager;

namespace UserManagement.ResourceProviders
{
    public class UserProvider
    {
        private readonly IUserManager _userManager;
        private readonly ILogger<UserProvider> _logger;

        public UserProvider(IUserManager userManager, ILogger<UserProvider> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [FunctionName(nameof(GetUser))]
        public async Task<IActionResult> GetUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/{userId}")]
            HttpRequest req,
            string userId)
        {
            _logger.LogInformation($"Starting {nameof(GetUser)} Function...");

            if (string.IsNullOrEmpty(userId))
            {
                return new BadRequestObjectResult("Missing userId");
            };

            AbacUser user = (await _userManager.GetUser(userId))?.Properties?.Properties;

            if (user is null)
            {
                return new NotFoundObjectResult($"User {userId} not found");
            }

            return new OkObjectResult(user);
        }

        [FunctionName(nameof(CreateUser))]
        public async Task<IActionResult> CreateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users")]
            HttpRequest req)
        {
            _logger.LogInformation($"Starting {nameof(CreateUser)} Function...");

            var requestBody = JsonConvert.DeserializeObject<AbacUser>(await req.ReadAsStringAsync());

            if (requestBody is null)
            {
                return new BadRequestObjectResult("Invalid request body");
            }

            AbacUser user = (await _userManager.CreateUser(requestBody))?.Properties?.Properties;

            return new OkObjectResult(user);
        }

        [FunctionName(nameof(UpdateUserAttributes))]
        public async Task<IActionResult> UpdateUserAttributes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "users/{userId}")]
            HttpRequest req,
            string userId)
        {
            _logger.LogInformation($"Starting {nameof(UpdateUserAttributes)} Function...");
            
            if (string.IsNullOrEmpty(userId))
            {
                return new BadRequestObjectResult("Missing userId.");
            };

            var newAttributes = JsonConvert.DeserializeObject<Dictionary<string, object>>(await req.ReadAsStringAsync());
            AbacUser user = (await _userManager.UpdateUserAttributes(userId, newAttributes))?.Properties?.Properties;

            if (user is null)
            {
                return new NotFoundObjectResult($"User {userId} not found.");
            }

            return new OkObjectResult(user);
        }

        [FunctionName(nameof(UpdateUserAttribute))]
        public async Task<IActionResult> UpdateUserAttribute(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "users/{userId}/attributes/{attributeName}")]
            HttpRequest req,
            string userId,
            string attributeName)
        {
            _logger.LogInformation($"Starting {nameof(UpdateUserAttribute)} Function...");
            var newAttribute = JsonConvert.DeserializeObject<KeyValuePair<string, object>>(await req.ReadAsStringAsync());

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(attributeName))
            {
                return new BadRequestObjectResult("Missing userId or attributeName.");
            };

            AbacUser user = (await _userManager.UpdateUserAttribute(userId, attributeName, newAttribute.Value))?.Properties?.Properties;

            if (user is null)
            {
                return new NotFoundObjectResult($"User {userId} not found.");
            }

            return new OkObjectResult(user);
        }

        [FunctionName(nameof(DeleteUserAttribute))]
        public async Task<IActionResult> DeleteUserAttribute(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "users/{userId}/attributes/{attributeName}")]
            HttpRequest req,
            string userId,
            string attributeName)
        {
            _logger.LogInformation($"Starting {nameof(DeleteUserAttribute)} Function...");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(attributeName))
            {
                return new BadRequestObjectResult("Missing userId or attributeName");
            };

            AbacUser user = (await _userManager.DeleteUserAttribute(userId, attributeName))?.Properties?.Properties;
            
            if (user is null)
            {
                return new NotFoundObjectResult($"User {userId} not found.");
            }

            return new OkObjectResult(user);
        }
    }
}
