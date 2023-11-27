using Common.Cosmos.Models.Entities;
using Common.Cosmos.Models.Properties;

namespace Common.Application.Converters
{
    public class CosmosDataConverter : ICosmosDataConverter
    {
        public dynamic FromEntity<T>(AbacEntity<T> source, Type type) where T : CosmosProperties
        {
            return Convert.ChangeType(source?.Properties, type);
        }
    }
}
