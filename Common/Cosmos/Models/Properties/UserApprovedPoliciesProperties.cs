using Newtonsoft.Json;

namespace Common.Cosmos.Models.Properties
{
    public class UserApprovedPoliciesProperties : CosmosProperties
    {
        [JsonProperty("userId", Required = Required.Always)]
        public string UserId { get; private set; }

        [JsonProperty("approvedPolicyIds", Required = Required.Always)]
        public IDictionary<string, DateTime> ApprovedPolicies { get; set; }

        public UserApprovedPoliciesProperties(string userId, IDictionary<string, DateTime> approvedPolicies, Guid systemId) : base(systemId)
        {
            UserId = userId;
            ApprovedPolicies = approvedPolicies;
        }
    }
}
