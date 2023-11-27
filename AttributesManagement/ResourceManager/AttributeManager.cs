using Common.Application.Models.API;
using Common.Application;
using Common.Cosmos.Models.Entities;
using Common.Cosmos.Models.Properties;
using Common.Cosmos.Providers;
using System.Threading.Tasks;

namespace AttributesManagement.ResourceManager
{
    public class AttributeManager : IAttributeManager
    {
        private readonly IAttributesContainerProvider _attributesProvider;

        public AttributeManager(IAttributesContainerProvider attributesProvider)
        {
            _attributesProvider = attributesProvider;
        }

        public async Task<UserAttributeEntity> GetAttribute(string attributeName)
        {
            return (await _attributesProvider.GetAttribute(attributeName))?.Resource;
        }

        public async Task<UserAttributeEntity> CreateAttribute(UserAttribute attribute)
        {
            UserAttributeProperties attributeProperties = new(attribute, SystemIdGenerator.GenerateId(attribute.Name));

            return (await _attributesProvider.CreateAttribute(attributeProperties, true))?.Resource;
        }
    }
}
