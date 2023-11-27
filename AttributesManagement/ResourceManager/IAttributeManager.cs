using Common.Application.Models.API;
using Common.Cosmos.Models.Entities;
using System.Threading.Tasks;

namespace AttributesManagement.ResourceManager
{
    /// <summary>
    /// AttributeManager is responsible for managing attributes in the system.
    /// </summary>
    public interface IAttributeManager
    {
        /// <summary>
        /// Gets an attribute by attribute's name.
        /// </summary> 
        Task<UserAttributeEntity> GetAttribute(string attributeName);

        /// <summary>
        /// Responsible for creating an attribute in the system.
        /// </summary>      
        Task<UserAttributeEntity> CreateAttribute(UserAttribute attribute);
    }
}
