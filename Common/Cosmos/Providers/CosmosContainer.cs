using Common.Cosmos.Models.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Net;

namespace Common.Cosmos.Providers
{
    public class CosmosContainer : ICosmosContainer
    {
        private readonly Container _container;
        private readonly ILogger _logger;

        public CosmosContainer(Container container, ILogger logger)
        {
            _container = container;
            _logger = logger;
        }

        public async Task<ItemResponse<TItem>> GetItem<TItem>(string id, string partitionKey)
        {
            ItemResponse<TItem> item = null;
            try
            {
                item = await _container.ReadItemAsync<TItem>(id, new PartitionKey(partitionKey));
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation($"Item: {id}, Pk: {partitionKey} was not found. Cost: {ex.Headers.RequestCharge}");
                item = null;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, $"Failed to read item id: {id}, pk: {partitionKey}. Error: {ex.Message}");

                throw;
            }

            return item;
        }

        public async Task<FeedResponse<TItem>> GetItems<TItem>(IList<(string Id, string PartitionKey)> ids)
        {
            FeedResponse<TItem> response = null;
            ReadOnlyCollection<(string Id, PartitionKey)> items = ids.Select(tuple => (tuple.Id, new PartitionKey(tuple.PartitionKey))).ToList().AsReadOnly();

            try
            {
                response = await _container.ReadManyItemsAsync<TItem>(items);

                if (response.Count != ids.Count)
                {
                    _logger.LogWarning($"Executing {nameof(Container.ReadManyItemsAsync)} returned {response.Count} out of {ids.Count} ids.");
                }
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, $"Failed executing {nameof(Container.ReadManyItemsAsync)}. Error: {ex.Message}");

                throw;
            }

            return response;
        }

        public Task<ItemResponse<TItem>> UpsertItem<TItem>(TItem item, bool returnResponse = false) where TItem : CosmosEntity
        {
            return ExecuteOperation(
                item,
                item => _container.UpsertItemAsync(
                    item,
                    new PartitionKey(item.PartitionKey),
                    new ItemRequestOptions
                    {
                        IfMatchEtag = item.ETag,
                        EnableContentResponseOnWrite = returnResponse
                    }));
        }

        public Task<ItemResponse<TItem>> UpdateItem<TItem>(TItem item, bool returnResponse = false) where TItem : CosmosEntity
        {
            return ExecuteOperation(
                item,
                item => _container.ReplaceItemAsync(
                    item,
                    item.Id,
                    new PartitionKey(item.PartitionKey),
                    new ItemRequestOptions
                    {
                        IfMatchEtag = item.ETag,
                        EnableContentResponseOnWrite = returnResponse
                    }));
        }

        public Task<FeedResponse<TItem>> ExecuteQuery<TItem>(string query, string partitionKey) where TItem : CosmosEntity
        {
            var requestOptions = new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) };
            using FeedIterator<TItem> iterator = _container.GetItemQueryIterator<TItem>(query, requestOptions: requestOptions);
            
            return ReadPage(iterator);
        }

        public IOrderedQueryable<TItem> GetItemLinqQueryable<TItem>() where TItem : CosmosEntity
        {
            return _container.GetItemLinqQueryable<TItem>();
        }

        private async Task<FeedResponse<TItem>> ReadPage<TItem>(FeedIterator<TItem> iterator) where TItem : CosmosEntity
        {
            try
            {
                FeedResponse<TItem> page = await iterator.ReadNextAsync();
                return page.StatusCode == HttpStatusCode.OK ? page : null;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, $"Failed executing query. Error: {ex.Message}");

                throw;
            }
        }

        private async Task<ItemResponse<TItem>> ExecuteOperation<TItem>(TItem item, Func<TItem, Task<ItemResponse<TItem>>> operationToRun)
            where TItem : CosmosEntity
        {
            ItemResponse<TItem> response = null;
            try
            {
                response = await operationToRun(item);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation($"Item: {item.Id}, Pk: {item.PartitionKey} was not found. Cost: {ex.Headers.RequestCharge}");
                response = null;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, $"Failed to process item id: {item.Id} pk: {item.PartitionKey}. Error: {ex.Message}");

                throw;
            }

            return response;
        }

    }
}
