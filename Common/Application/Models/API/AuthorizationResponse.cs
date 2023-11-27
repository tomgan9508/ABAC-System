using Newtonsoft.Json;

namespace Common.Application.Models.API
{
    public class AuthorizationResponse
    {
        [JsonProperty("isAuthorized", Required = Required.Always)]
        public bool IsAuthorized { get; private set; }

        public AuthorizationResponse(bool isAuthorized)
        {
            IsAuthorized = isAuthorized;
        }

        [JsonConstructor]
        private AuthorizationResponse()
        {
        }
    }
}
