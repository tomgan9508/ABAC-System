using Newtonsoft.Json;

namespace Common.Application.Models.API
{
    public sealed class AbacResource
    {
        [JsonProperty("resourceId", Required = Required.Always)]
        public string Id { get; private set; }

        [JsonProperty("policyIds", Required = Required.Always)]
        public ISet<string> Policies { get; set; }

        public AbacResource(string id)
        {
            Id = id;
        }

        [JsonConstructor]
        private AbacResource()
        {
        }
    }
}
