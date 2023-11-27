using Common.Application.Models.API;
using Newtonsoft.Json;

namespace Common.Cosmos.Models.Properties
{
    public class PolicyProperties : CosmosProperties
    {
        [JsonProperty("properties", Required = Required.Always)]
        public Policy Properties { get; set; }

        public PolicyProperties(Policy properties, Guid systemId) : base(systemId)
        {
            Properties = properties;
        }
    }
}
