using Newtonsoft.Json;

namespace Common.Application.Models.API
{
    public class UserAttribute
    {
        [JsonProperty("attributeName", Required = Required.Always)]
        public string Name { get; private set; }

        [JsonProperty("attributeType", Required = Required.Always)]
        public string Type { get; private set; }

        public UserAttribute(string name, string type)
        {
            Name = name;
            Type = type;
        }

        [JsonConstructor]
        private UserAttribute()
        {
        }
    }
}