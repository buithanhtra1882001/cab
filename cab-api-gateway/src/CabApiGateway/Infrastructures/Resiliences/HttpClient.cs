using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace GatewayApi.Infrastructures.Resiliences
{
    public class HttpClient : IHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetStringAsync(string uri)
        {
            var response = await GetHttpClient().GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return string.Empty;
        }

        public async Task<T> PostAsync<T>(string uri, HttpContent data)
        {
            var response = await GetHttpClient().PostAsync(uri, data);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            return default;
        }

        private System.Net.Http.HttpClient GetHttpClient()
        {
            return _httpClientFactory.CreateClient();
        }
    }
}