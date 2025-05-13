using Amazon.Runtime.Internal.Transform;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;

namespace CabMediaService.Integration.Dropbox
{
    public class DropboxService : IDropboxService
    {
        private DateTime accessTokenExpiryTime;
        private string accessToken;
        private string refreshToken;
        private readonly DropboxSettings _dropboxSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        public DropboxService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _dropboxSettings = configuration.GetSection(nameof(DropboxSettings)).Get<DropboxSettings>();
            _httpClientFactory = httpClientFactory;
        }

        public string GetUploadVideoPath() => _dropboxSettings.UploadVideoPath;

        public string GetUploadImagePath() => _dropboxSettings.UploadImagePath;

        public async Task<string> GetToken()
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", _dropboxSettings.RefreshToken },
                    { "client_id", _dropboxSettings.AppKey },
                    { "client_secret", _dropboxSettings.AppSecret },
                };

                var content = new FormUrlEncodedContent(values);
                HttpResponseMessage response = await client.PostAsync(_dropboxSettings.UrlGetToken, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(responseContent);
                    if (jsonDocument.RootElement.TryGetProperty("access_token", out JsonElement accessTokenElement))
                        return accessTokenElement.GetString();
                    
                    throw new Exception("Failed to retrieve access token.");
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to get token. Status Code: {response.StatusCode}, Response: {errorContent}");
            }
        }
    }
}
