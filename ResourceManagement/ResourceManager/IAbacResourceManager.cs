using Common.Application.Models.API;
using Common.Cosmos.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResourceManagement.ResourceManager
{
    /// <summary>
    /// ResourceManager is responsible for managing resources in the system.
    /// </summary>
    public interface IAbacResourceManager
    {
        /// <summary>
        /// Gets a resource by id.
        /// </summary>    
        Task<AbacResourceEntity> GetResource(string resourceId);

        /// <summary>
        /// Responsible for creating a resource in the system.
        /// </summary>    
        Task<AbacResourceEntity> CreateResource(AbacResource resource);

        /// <summary>
        /// Responsible for updating a resource's policies in the system.
        /// </summary>    
        Task<AbacResourceEntity> UpdateResourcePolicies(string resourceId, IEnumerable<string> policyIds);
    }
}
