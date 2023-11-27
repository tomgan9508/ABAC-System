using Newtonsoft.Json;

namespace Common.Cosmos.Models.Properties
{
    public abstract class CosmosProperties
    {
        [JsonProperty("systemId", Required = Required.Always)]
        public Guid SystemId { get; set; }

        [JsonProperty("lastUpdated", Required = Required.Always)]
        public DateTime LastUpdated { get; private set; }

        [JsonProperty("created", Required = Required.Always)]
        public DateTime Created { get; private set; }

        protected CosmosProperties(Guid systemId)
        {
            SystemId = systemId;
            Created = DateTime.UtcNow;
            LastUpdated = DateTime.UtcNow;
        }

        [JsonConstructor]
        private CosmosProperties()
        {
        }

        public void UpdateLastUpdated()
        {
            LastUpdated = DateTime.UtcNow;
        }
    }
}
