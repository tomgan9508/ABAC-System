using Common.Application;
using Common.Cosmos.Models.Properties;
using Newtonsoft.Json;

namespace Common.Cosmos.Models.Entities
{
    public class UserAttributeEntity : AbacEntity<UserAttributeProperties>
    {
        public const string EntityType = "Attribute";

        public UserAttributeEntity(UserAttributeProperties attribute) : base(GenerateId(attribute.Properties.Name),
                                                                             GeneratePartitionKey(attribute.Properties.Name),
                                                                             EntityType, attribute)
        {
        }

        [JsonConstructor]
        private UserAttributeEntity()
        {
        }

        public static string GenerateId(string attributeName)
        {
            return SystemIdGenerator.GenerateId(attributeName).ToString();
        }

        public static string GeneratePartitionKey(string attributeName)
        {
            return SystemIdGenerator.GenerateId(attributeName).ToString();
        }
    }
}
