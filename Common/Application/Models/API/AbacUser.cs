using Newtonsoft.Json;

namespace Common.Application.Models.API
{
    public sealed class AbacUser
    {
        [JsonProperty("userId", Required = Required.Always)]
        public string Id { get; private set; }

        [JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, object> Attributes { get; set; }

        public AbacUser(string id)
        {
            Id = id;
        }

        [JsonConstructor]
        private AbacUser()
        {
        }
    }
}
