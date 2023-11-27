using Common.Cosmos.Models.Entities;
using Common.Cosmos.Models.Properties;

namespace Common.Application.Converters
{
    public interface ICosmosDataConverter
    {
        dynamic FromEntity<T>(AbacEntity<T> source, Type type) where T : CosmosProperties;
    }
}
