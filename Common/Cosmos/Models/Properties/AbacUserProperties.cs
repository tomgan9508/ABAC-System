using Common.Application.Models.API;
using Newtonsoft.Json;

namespace Common.Cosmos.Models.Properties
{
    public class AbacUserProperties : CosmosProperties
    {
        [JsonProperty("properties", Required = Required.Always)]
        public AbacUser Properties { get; set; }

        public AbacUserProperties(AbacUser properties, Guid systemId) : base(systemId)
        {
            Properties = properties;
        }
    }
}
