using Common.Cosmos.Models.Entities;
using Common.Cosmos.Models.Properties;
using Microsoft.Azure.Cosmos;

namespace Common.Cosmos.Providers
{
    /// <summary>
    /// UsersContainerProvider handles all the entities saved in the Users container and manages
    /// the operations againt the database.
    /// This container will store all users related entities.
    /// </summary>
    public interface IUsersContainerProvider
    {
        /// <summary>
        /// Gets a user from cosmos
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <returns>The User Entity from cosmos</returns>
        Task<ItemResponse<AbacUserEntity>> GetUser(string userId);

        /// <summary>
        /// Creates a user in cosmos
        /// </summary>
        /// <param name="user">The user's properties</param>
        /// <param name="returnResponse">Flag that indicates if to return cosmos full response or not</param>
        /// <returns>The created User Entity from cosmos</returns>
        Task<ItemResponse<AbacUserEntity>> CreateUser(AbacUserProperties user, bool returnResponse = false);

        /// <summary>
        /// Updates a user in cosmos
        /// </summary>
        /// <param name="user">The user's properties</param>
        /// <param name="returnResponse">Flag that indicates if to return cosmos full response or not</param>
        /// <returns>The updated User Entity from cosmos</returns>
        Task<ItemResponse<AbacUserEntity>> UpdateUser(AbacUserProperties user, bool returnResponse = false);

        /// <summary>
        /// Gets a user related info entity from cosmos by user partition key
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <returns>The relevant user info Entity from cosmos</returns>
        Task<FeedResponse<T>> GetUserInfoByPartition<T>(string userId) where T : CosmosEntity;

        /// <summary>
        /// Creates or updates a user approved policies entity in cosmos
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <param name="approvedPolicies">The user's approved policies</param>
        /// <param name="returnResponse">Flag that indicates if to return cosmos full response or not</param>
        /// <returns>The created / updated user approved policies Entity from cosmos</returns>
        Task<ItemResponse<UserApprovedPoliciesEntity>> CreateOrUpdateUserApprovedPolicies(string userId, IDictionary<string, DateTime> approvedPolicies, bool returnResponse = false);

        /// <summary>
        /// Gets a user approved policies entity from cosmos
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <returns>The user approved policies Entity from cosmos</returns>
        Task<ItemResponse<UserApprovedPoliciesEntity>> GetUserApprovedPolicies(string userId);

        /// <summary>
        /// Adds a policy to the user's approved policies entity in cosmos
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <param name="policyId">The policy to add</param>
        /// <returns>The updated user approved policies Entity from cosmos</returns>
        Task<ItemResponse<UserApprovedPoliciesEntity>> AddUserApprovedPolicy(string userId, string policyId);
    }
}
