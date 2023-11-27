using Common.Application.Models.API;
using Common.Cosmos.Models.Entities;
using Common.Cosmos.Models.Properties;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PolicyManagement.ResourceManager
{
    /// <summary>
    /// PolicyManager is responsible for managing policies in the system.
    /// </summary>
    public interface IPolicyManager
    {
        /// <summary>
        /// Gets a policy by id.
        /// </summary>    
        Task<PolicyEntity> GetPolicy(string policyId);

        /// <summary>
        /// Responsible for creating a policy in the system.
        /// </summary>     
        Task<PolicyEntity> CreatePolicy(Policy policy);

        /// <summary>
        /// Responsible for updating a policy's conditions in the system.
        /// </summary>      
        Task<PolicyEntity> UpdatePolicyConditions(string policyId, IEnumerable<PolicyCondition> newConditions);
    }
}
