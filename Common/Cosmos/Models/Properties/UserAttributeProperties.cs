using Common.Application.Models.API;
using Newtonsoft.Json;

namespace Common.Cosmos.Models.Properties
{
    public class UserAttributeProperties : CosmosProperties
    {
        [JsonProperty("properties", Required = Required.Always)]
        public UserAttribute Properties { get; set; }

        public UserAttributeProperties(UserAttribute properties, Guid systemId) : base(systemId)
        {
            Properties = properties;
        }
    }
}
