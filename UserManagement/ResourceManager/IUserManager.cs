using Common.Application.Models.API;
using Common.Cosmos.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserManagement.ResourceManager
{
    /// <summary>
    /// UserManager is responsible for managing users in the system.
    /// </summary>
    public interface IUserManager
    {
        /// <summary>
        /// Gets a user by id.
        /// </summary>        
        Task<AbacUserEntity> GetUser(string userId);

        /// <summary>
        /// Responsible for creating a user in the system.
        /// </summary>        
        Task<AbacUserEntity> CreateUser(AbacUser user);

        /// <summary>
        /// Responsible for updating a user's attribute in the system.
        /// </summary>      
        Task<AbacUserEntity> UpdateUserAttribute(string userId, string attributeName, object attributeValue);

        /// <summary>
        /// Responsible for updating a user's attributes in the system.
        /// </summary>      
        Task<AbacUserEntity> UpdateUserAttributes(string userId, IDictionary<string, object> newAttributes);

        /// <summary>
        /// Responsible for deleting a user's attribute in the system.
        /// </summary>      
        Task<AbacUserEntity> DeleteUserAttribute(string userId, string attributeName);

    }
}
