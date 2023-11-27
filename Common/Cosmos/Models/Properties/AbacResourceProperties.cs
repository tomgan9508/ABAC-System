using Common.Application.Models.API;
using Newtonsoft.Json;

namespace Common.Cosmos.Models.Properties
{
    public sealed class AbacResourceProperties : CosmosProperties
    {
        [JsonProperty("properties", Required = Required.Always)]
        public AbacResource Properties { get; set; }

        public AbacResourceProperties(AbacResource properties, Guid systemId) : base(systemId)
        {
            Properties = properties;
        }
    }
}
