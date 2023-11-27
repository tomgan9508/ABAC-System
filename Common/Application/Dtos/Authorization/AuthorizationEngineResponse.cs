using Newtonsoft.Json;

namespace Common.Application.Dtos.Authorization
{
    public class AuthorizationEngineResponse
    {
        [JsonProperty("containsFulfilledPolicy", Required = Required.Always)]
        public bool ContainsFulfilledPolicy { get; set; }

        [JsonProperty("policyFulfilledId", Required = Required.Always)]
        public string PolicyFulfilledId { get; set; }

        [JsonConstructor]
        public AuthorizationEngineResponse(bool containsFulfilledPolicy = false, string policyFulfilledId = "")
        {
            ContainsFulfilledPolicy = containsFulfilledPolicy;
            PolicyFulfilledId = policyFulfilledId;
        }
    }
}
