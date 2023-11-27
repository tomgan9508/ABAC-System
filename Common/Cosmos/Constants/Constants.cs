namespace Common.Cosmos.Constants
{
    public static class Constants
    {
        public const string TypePropertyName = "type";
        public const string PartitionKeyPropertyName = "pk";
        public const string CosmosDatabaseName = "AbacSystem";

        // Containers
        public const string UsersContainerName = "users";
        public const string PoliciesContainerName = "policies";
        public const string AttributesContainerName = "attributes";
        public const string ResourcesContainerName = "resources";

        // local connection string
        public const string LocalAccountEndpoint = "https://localhost:8081/";
        public const string LocalAuthKeyOrResourceToken = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
    }
}
