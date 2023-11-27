using Newtonsoft.Json;

namespace Common.Application.Dtos.Authorization
{
    public class PreAuthorizationEngineRequest
    {
        [JsonProperty("policyIds", Required = Required.Always)]
        public IEnumerable<string> ResourcePolicyIds { get; set; }

        [JsonProperty("userLastUpdated", Required = Required.Always)]
        public DateTime UserLastUpdated { get; set; }

        [JsonProperty("userApprovedPolicies", Required = Required.Always)]
        public IDictionary<string, DateTime> UserApprovedPolicies { get; set; }

        public PreAuthorizationEngineRequest(
            IEnumerable<string> resourcePolicyIds,
            DateTime userLastUpdated, 
            IDictionary<string, DateTime> userApprovedPolicies)
        {
            UserLastUpdated = userLastUpdated;
            ResourcePolicyIds = resourcePolicyIds;
            UserApprovedPolicies = userApprovedPolicies;
        }

        [JsonConstructor]
        private PreAuthorizationEngineRequest()
        {
        }
    }
}
