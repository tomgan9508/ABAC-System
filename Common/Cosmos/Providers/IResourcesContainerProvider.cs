using Common.Cosmos.Models.Entities;
using Common.Cosmos.Models.Properties;
using Microsoft.Azure.Cosmos;

namespace Common.Cosmos.Providers
{
    /// <summary>
    /// ResourcesContainerProvider handles all the entities saved in the Resources container and manages
    /// the operations againt the database.
    /// This container will store all resources related entities.
    /// </summary>
    public interface IResourcesContainerProvider
    {
        /// <summary>
        /// Gets a resource from cosmos
        /// </summary>
        /// <param name="resourceId">The resource's Id</param>
        /// <returns>The resource Entity from cosmos</returns>
        Task<ItemResponse<AbacResourceEntity>> GetResource(string resourceId);

        /// <summary>
        /// Creates a resource in cosmos
        /// </summary>
        /// <param name="resource">The resource's properties</param>
        /// <param name="returnResponse">Flag that indicates if to return cosmos full response or not</param>
        /// <returns>The created resource Entity from cosmos</returns>
        Task<ItemResponse<AbacResourceEntity>> CreateResource(AbacResourceProperties resource, bool returnResponse = false);

        /// <summary>
        /// Updates a resource in cosmos
        /// </summary>
        /// <param name="resource">The resource's properties</param>
        /// <param name="returnResponse">Flag that indicates if to return cosmos full response or not</param>
        /// <returns>The updated resource Entity from cosmos</returns>
        Task<ItemResponse<AbacResourceEntity>> UpdateResource(AbacResourceProperties resource, bool returnResponse = false);
    }
}
