using Common.Cosmos.Models.Properties;
using Newtonsoft.Json;

namespace Common.Application.Models.API
{
    public class Policy
    {
        [JsonProperty("policyId", Required = Required.Always)]
        public string Id { get; private set; }

        [JsonProperty("conditions", Required = Required.Always)]
        public IList<PolicyCondition> Conditions { get; set; }

        public Policy(string id)
        {
            Id = id;
        }

        [JsonConstructor]
        private Policy()
        {
        }
    }
}
