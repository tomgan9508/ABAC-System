using Newtonsoft.Json;

namespace Common.Cosmos.Models.Properties
{
    public sealed class PolicyCondition
    {
        [JsonProperty("attributeName", Required = Required.Always)]
        public string AttributeName { get; private set; }

        [JsonProperty("operator", Required = Required.Always)]
        public string Operator { get; private set; }

        [JsonProperty("value", Required = Required.Always)]
        public object Value { get; private set; }

        public PolicyCondition(string attributeName, string op, string value)
        {
            AttributeName = attributeName;
            Operator = op;
            Value = value;
        }

        [JsonConstructor]
        private PolicyCondition()
        {
        }
    }
}
