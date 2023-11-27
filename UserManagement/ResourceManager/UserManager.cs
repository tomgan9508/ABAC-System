using Common.Application;
using Common.Application.Models.API;
using Common.Cosmos.Models.Entities;
using Common.Cosmos.Models.Properties;
using Common.Cosmos.Providers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserManagement.ResourceManager
{
    public class UserManager : IUserManager
    {
        private readonly IUsersContainerProvider _userProvider;

        public UserManager(IUsersContainerProvider userProvider)
        {
            _userProvider = userProvider;
        }

        public async Task<AbacUserEntity> GetUser(string userId)
        {
            return (await _userProvider.GetUser(userId))?.Resource;
        }

        public async Task<AbacUserEntity> CreateUser(AbacUser user)
        {
            AbacUserProperties userProperties = new(user, SystemIdGenerator.GenerateId(user.Id));

            return (await _userProvider.CreateUser(userProperties, true))?.Resource;
        }

        public async Task<AbacUserEntity> UpdateUserAttribute(string userId, string attributeName, object attributeValue)
        {
            AbacUserEntity entity = await GetUser(userId);
            if (entity is null)
            {
                return null;
            }

            AbacUserProperties user = entity.Properties;
            if (user.Properties.Attributes.ContainsKey(attributeName))
            {
                user.Properties.Attributes[attributeName] = attributeValue;
            }
            else
            {
                user.Properties.Attributes.Add(attributeName, attributeValue);
            }

            return await _userProvider.UpdateUser(user, true);
        }

        public async Task<AbacUserEntity> UpdateUserAttributes(
            string userId,
            IDictionary<string, object> newAttributes)
        {
            AbacUserEntity entity = await GetUser(userId);
            if (entity is null)
            {
                return null;
            }

            entity.Properties.Properties.Attributes = newAttributes;
            return await _userProvider.UpdateUser(entity.Properties, true);
        }

        public async Task<AbacUserEntity> DeleteUserAttribute(string userId, string attributeName)
        {
            AbacUserEntity entity = await GetUser(userId);
            if (entity is null)
            {
                return null;
            }

            entity.Properties.Properties.Attributes.Remove(attributeName);

            return await _userProvider.UpdateUser(entity.Properties, true);
        }
    }
}
