using Common.Application;
using Common.Cosmos.Models.Properties;
using Newtonsoft.Json;

namespace Common.Cosmos.Models.Entities
{
    public class AbacResourceEntity : AbacEntity<AbacResourceProperties>
    {
        public const string EntityType = "Resource";

        public AbacResourceEntity(AbacResourceProperties resource) : base(GenerateId(resource.Properties.Id),
                                                                          GeneratePartitionKey(resource.Properties.Id),
                                                                          EntityType, 
                                                                          resource)
        {
        }

        [JsonConstructor]
        private AbacResourceEntity()
        {
        }

        public static string GenerateId(string resourceId)
        {
            return SystemIdGenerator.GenerateId(resourceId).ToString();
        }

        public static string GeneratePartitionKey(string resourceId)
        {
            return SystemIdGenerator.GenerateId(resourceId).ToString();
        }
    }
}
