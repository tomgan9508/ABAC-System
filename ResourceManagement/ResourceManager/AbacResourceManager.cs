using Common.Application;
using Common.Application.Models.API;
using Common.Cosmos.Models.Entities;
using Common.Cosmos.Models.Properties;
using Common.Cosmos.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceManagement.ResourceManager
{
    public class AbacResourceManager : IAbacResourceManager
    {
        private readonly IResourcesContainerProvider _resourceProvider;

        public AbacResourceManager(IResourcesContainerProvider resourceProvider)
        {
            _resourceProvider = resourceProvider;
        }

        public async Task<AbacResourceEntity> GetResource(string resourceId)
        {
            return (await _resourceProvider.GetResource(resourceId))?.Resource;
        }

        public async Task<AbacResourceEntity> CreateResource(AbacResource resource)
        {
            AbacResourceProperties resourceProperties = new(resource, SystemIdGenerator.GenerateId(resource.Id));

            return (await _resourceProvider.CreateResource(resourceProperties, true))?.Resource;
        }

        public async Task<AbacResourceEntity> UpdateResourcePolicies(string resourceId, IEnumerable<string> policyIds)
        {
            AbacResourceEntity entity = await GetResource(resourceId);

            if (entity is null)
            {
                return null;
            }

            entity.Properties.Properties.Policies = policyIds.ToHashSet();

            return await _resourceProvider.UpdateResource(entity.Properties, true);
        }

    }
}
