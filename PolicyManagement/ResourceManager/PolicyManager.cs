using Common.Application;
using Common.Application.Models.API;
using Common.Cosmos.Models.Entities;
using Common.Cosmos.Models.Properties;
using Common.Cosmos.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolicyManagement.ResourceManager
{
    public class PolicyManager : IPolicyManager
    {
        private readonly IPoliciesContainerProvider _policyProvider;

        public PolicyManager(IPoliciesContainerProvider policyProvider)
        {
            _policyProvider = policyProvider;
        }

        public async Task<PolicyEntity> GetPolicy(string policyId)
        {
            return (await _policyProvider.GetPolicy(policyId))?.Resource;
        }


        public async Task<PolicyEntity> CreatePolicy(Policy policy)
        {
            PolicyProperties policyProperties = new(policy, SystemIdGenerator.GenerateId(policy.Id));

            return (await _policyProvider.CreatePolicy(policyProperties, true))?.Resource;
        }


        public async Task<PolicyEntity> UpdatePolicyConditions(string policyId, IEnumerable<PolicyCondition> newConditions)
        {
            PolicyEntity entity = await GetPolicy(policyId);
            if (entity is null)
            {
                return null;
            }

            entity.Properties.Properties.Conditions = newConditions.ToList();
            return await _policyProvider.UpdatePolicy(entity.Properties, true);
        }
    }
}
