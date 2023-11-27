using Common.Cosmos.Models.Entities;
using Common.Cosmos.Models.Properties;
using Microsoft.Azure.Cosmos;

namespace Common.Cosmos.Providers
{
    /// <summary>
    /// AttributesContainerProvider handles all the entities saved in the Attributes container and manages
    /// the operations againt the database.
    /// This container will store all attributes related entities.
    /// </summary>
    public interface IAttributesContainerProvider
    {
        /// <summary>
        /// Creates an attribute in cosmos
        /// </summary>
        /// <param name="attribute">The attribute's properties</param>
        /// <param name="returnResponse">Flag that indicates if to return cosmos full response or not</param>
        /// <returns>The created attribute Entity from cosmos</returns>
        Task<ItemResponse<UserAttributeEntity>> CreateAttribute(UserAttributeProperties attribute, bool returnResponse = false);

        /// <summary>
        /// Gets an attribute from cosmos
        /// </summary>
        /// <param name="attributeName">The attribute's name</param>
        /// <returns>The attribute Entity from cosmos</returns>
        Task<ItemResponse<UserAttributeEntity>> GetAttribute(string attributeName);

        /// <summary>
        /// Gets attributes from cosmos
        /// </summary>
        /// <param name="attributeNames">The attribute names</param>
        /// <returns>The attribute Entities from cosmos</returns>
        Task<FeedResponse<UserAttributeEntity>> GetAttributes(IEnumerable<string> attributeNames);

    }
}
