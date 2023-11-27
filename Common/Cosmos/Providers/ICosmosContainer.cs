using Common.Cosmos.Models.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Cosmos.Providers
{
    /// <summary>
    /// CosmosContainer handles and manages all the 
    /// operations againt the database using the cosmos client.
    /// </summary>
    public interface ICosmosContainer
    {
        /// <summary>
        /// Gets an item from cosmos
        /// <param name="id">The entity's Id</param>
        /// <param name="partitionKey">The entity's partition key</param>
       /// </summary>
        Task<ItemResponse<TItem>> GetItem<TItem>(string id, string partitionKey);

        /// <summary>
        /// Gets items from cosmos
        /// <param name="ids">List of tuples of the item Ids and partition keys</param>
        /// </summary>
        Task<FeedResponse<TItem>> GetItems<TItem>(IList<(string Id, string PartitionKey)> ids);

        /// <summary>
        /// Upserts an item into cosmos
        /// <param name="item">The item to upsert</param>
        /// <param name="returnResponse">Flag that indicates if to return cosmos full response or not</param>
        /// </summary>
        Task<ItemResponse<TItem>> UpsertItem<TItem>(TItem item, bool returnResponse = false) where TItem : CosmosEntity;

        /// <summary>
        /// Updates an item in cosmos
        /// <param name="item">The item to update</param>
        /// <param name="returnResponse">Flag that indicates if to return cosmos full response or not</param>
        /// </summary>
        Task<ItemResponse<TItem>> UpdateItem<TItem>(TItem item, bool returnResponse = false) where TItem : CosmosEntity;

        /// <summary>
        /// Returns a cosmos page of items using a specific query.
        /// <param name="partitionKey">The entity's partition key</param>
        /// <param name="query">The query to use against cosmos</param>
        /// </summary>
        Task<FeedResponse<TItem>> ExecuteQuery<TItem>(string query, string partitionKey) where TItem : CosmosEntity;

        IOrderedQueryable<TItem> GetItemLinqQueryable<TItem>() where TItem : CosmosEntity;
    }
}
