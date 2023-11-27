using Common.Application;
using Common.Cosmos.Models.Properties;
using Newtonsoft.Json;

namespace Common.Cosmos.Models.Entities
{
    public class PolicyEntity : AbacEntity<PolicyProperties>
    {
        public const string EntityType = "Policy";

        public PolicyEntity(PolicyProperties policy) : base(GenerateId(policy.Properties.Id),
                                                            GeneratePartitionKey(policy.Properties.Id),
                                                            EntityType, policy)
        {
        }

        [JsonConstructor]
        private PolicyEntity()
        {
        }

        public static string GenerateId(string policyId)
        {
            return SystemIdGenerator.GenerateId(policyId).ToString();
        }

        public static string GeneratePartitionKey(string policyId)
        {
            return SystemIdGenerator.GenerateId(policyId).ToString();
        }
    }
}
