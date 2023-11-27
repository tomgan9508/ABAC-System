using Newtonsoft.Json;

namespace Common.Cosmos.Models.Entities
{
    /// <summary>
    /// CosmosEntity is a base class for all cosmos entities.
    /// containning the entity's base properties (Id, pk etc.).
    /// </summary>
    public class CosmosEntity
    {
        [JsonProperty(Constants.Constants.PartitionKeyPropertyName, Required = Required.Always)]
        public string PartitionKey { get; set; }

        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_etag", NullValueHandling = NullValueHandling.Ignore)]
        public string ETag { get; set; }

        [JsonProperty(Constants.Constants.TypePropertyName, NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        public CosmosEntity(string id, string partitionKey, string type)
        {
            Id = id;
            PartitionKey = partitionKey;
            Type = type;
        }

        protected CosmosEntity()
        {
        }
    }
}
