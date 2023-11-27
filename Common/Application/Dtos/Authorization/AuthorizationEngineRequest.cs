using Newtonsoft.Json;

namespace Common.Application.Dtos.Authorization
{
    public class AuthorizationEngineRequest
    {
        [JsonProperty("policyIds", Required = Required.Always)]
        public IEnumerable<string> ResourcePolicyIds { get; set; }

        [JsonProperty("attributes", Required = Required.Always)]
        public IDictionary<string, object> Attributes { get; set; }

        public AuthorizationEngineRequest(IEnumerable<string> resourcePolicyIds, IDictionary<string, object> attributes)
        {
            ResourcePolicyIds = resourcePolicyIds ?? new List<string>();
            Attributes = attributes ?? new Dictionary<string, object>();
        }

        [JsonConstructor]
        private AuthorizationEngineRequest()
        {
        }
    }
}
