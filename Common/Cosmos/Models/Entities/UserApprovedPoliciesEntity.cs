using Common.Application;
using Common.Cosmos.Models.Properties;
using Newtonsoft.Json;

namespace Common.Cosmos.Models.Entities
{
    public class UserApprovedPoliciesEntity : AbacEntity<UserApprovedPoliciesProperties>
    {
        public const string EntityType = "UserApprovedPolicies";

        public UserApprovedPoliciesEntity(UserApprovedPoliciesProperties userPolicies) : base(GenerateId(userPolicies.UserId),
                                                                                              GeneratePartitionKey(userPolicies.UserId),
                                                                                              EntityType, userPolicies)
        {
        }

        [JsonConstructor]
        private UserApprovedPoliciesEntity()
        {
        }

        public static string GenerateId(string userId)
        {
            return $"{SystemIdGenerator.GenerateId(userId)}_{EntityType}";
        }

        public static string GeneratePartitionKey(string userId)
        {
            return SystemIdGenerator.GenerateId(userId).ToString();
        }
    }
}
