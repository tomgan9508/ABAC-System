using Newtonsoft.Json;

namespace Common.Application.Dtos.Authorization
{
    public class PreAuthorizationEngineResponse
    {
        [JsonProperty("fulfilledPolicie", Required = Required.Always)]
        public IEnumerable<string> UnExpiredPolicyApprovals { get; set; }

        [JsonProperty("unFulfilledPolicies", Required = Required.Always)]
        public IEnumerable<string> ExpiredPolicyApprovals { get; set; }

        [JsonConstructor]
        public PreAuthorizationEngineResponse()
        {
            UnExpiredPolicyApprovals = new List<string>();
            ExpiredPolicyApprovals = new List<string>();
        }
    }
}
