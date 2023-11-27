using Common.Application;
using Common.Cosmos.Models.Properties;
using Newtonsoft.Json;

namespace Common.Cosmos.Models.Entities
{
    public sealed class AbacUserEntity : AbacEntity<AbacUserProperties>
    {
        public const string EntityType = "User";

        public AbacUserEntity(AbacUserProperties user) : base(GenerateId(user.Properties.Id),
                                                              GeneratePartitionKey(user.Properties.Id),
                                                              EntityType, user)
        {
        }

        [JsonConstructor]
        private AbacUserEntity()
        {
        }

        public static string GenerateId(string userId)
        {
            return SystemIdGenerator.GenerateId(userId).ToString();
        }

        public static string GeneratePartitionKey(string userId)
        {
            return SystemIdGenerator.GenerateId(userId).ToString();
        }
    }
}
