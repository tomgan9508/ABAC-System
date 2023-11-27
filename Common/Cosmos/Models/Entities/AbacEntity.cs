using Newtonsoft.Json;

namespace Common.Cosmos.Models.Entities
{
    /// <summary>
    /// AbacEntity is a generic base class for all system's 
    /// cosmos related entities containning the entity's properties.
    /// </summary>
    public class AbacEntity<TEntity> : CosmosEntity
    {
        protected AbacEntity(string id, string partitionKey, string type, TEntity entity) : base(id, partitionKey, type)
        {
            Properties = entity;
        }

        protected AbacEntity()
        {
        }

        [JsonProperty("properties", NullValueHandling = NullValueHandling.Ignore)]
        public TEntity Properties { get; set; }
    }
}
