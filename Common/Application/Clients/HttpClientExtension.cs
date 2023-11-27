using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace Common.Application.Clients
{
    public class HttpClientExtension : IHttpClientExtension
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpClientExtension> _logger;

        public HttpClientExtension(IHttpClientFactory httpClientFactory, ILogger<HttpClientExtension> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<T> PostAsync<T>(Uri uri, string serializedBody)
        {
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = uri,
                Content = new StringContent(serializedBody, Encoding.UTF8, MediaTypeNames.Application.Json)
            };

            using HttpClient httpClient = _httpClientFactory.CreateClient();
            using HttpResponseMessage response = await httpClient.SendAsync(request);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                string errorResponseString = await response?.Content.ReadAsStringAsync();
                _logger.LogError(ex, $"Http request failed, statusCode=[{response?.StatusCode}], requestUri=[{request?.RequestUri}]");

                throw;
            }

            string responseStr = await response.Content.ReadAsStringAsync();
            T ret = JsonConvert.DeserializeObject<T>(responseStr);

            return ret;
        }

    }
}
