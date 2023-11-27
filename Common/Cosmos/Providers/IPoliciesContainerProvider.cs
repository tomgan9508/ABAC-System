using Common.Cosmos.Models.Entities;
using Common.Cosmos.Models.Properties;
using Microsoft.Azure.Cosmos;

namespace Common.Cosmos.Providers
{
    /// <summary>
    /// PoliciesContainerProvider handles all the entities saved in the Policies container and manages
    /// the operations againt the database.
    /// This container will store all policies related entities.
    /// </summary>
    public interface IPoliciesContainerProvider
    {
        /// <summary>
        /// Gets a policy from cosmos
        /// </summary>
        /// <param name="policyId">The policy's Id</param>
        /// <returns>The policy Entity from cosmos</returns>
        Task<ItemResponse<PolicyEntity>> GetPolicy(string policyId);

        /// <summary>
        /// Gets policies from cosmos
        /// </summary>
        /// <param name="policyIds">The policy Ids</param>
        /// <returns>The policy Entities from cosmos</returns>
        Task<FeedResponse<PolicyEntity>> GetPolicies(IEnumerable<string> policyIds);

        /// <summary>
        /// Creates a policy in cosmos
        /// </summary>
        /// <param name="policy">The policy's properties</param>
        /// <param name="returnResponse">Flag that indicates if to return cosmos full response or not</param>
        /// <returns>The created policy Entity from cosmos</returns>
        Task<ItemResponse<PolicyEntity>> CreatePolicy(PolicyProperties policy, bool returnResponse = false);

        /// <summary>
        /// Updates a policy in cosmos
        /// </summary>
        /// <param name="policy">The policy's properties</param>
        /// <param name="returnResponse">Flag that indicates if to return cosmos full response or not</param>
        /// <returns>The updated policy Entity from cosmos</returns>
        Task<ItemResponse<PolicyEntity>> UpdatePolicy(PolicyProperties policy, bool returnResponse = false);
    }
}
