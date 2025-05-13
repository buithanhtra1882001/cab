using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CabPostService.Infrastructures.Extensions;
using Newtonsoft.Json;

namespace CabPostService.Infrastructures.Communications.Http
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpClientWrapper> _logger;

        public HttpClientWrapper(HttpClient httpClient, ILogger<HttpClientWrapper> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        #region GET

        public async Task<HttpResponseMessage> GetAsync(string uri
            , AuthConfiguration authConfig = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var request = CreateRequest(HttpMethod.Get, uri, authConfig, headers: headers);
            return await SendAsync(request, cancellationToken);
        }

        public async Task<TRequest> GetAsync<TRequest>(string uri
            , AuthConfiguration authConfig = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var request = CreateRequest(HttpMethod.Get, uri, authConfig, headers: headers);
            var response = await SendAsync(request, cancellationToken, HttpCompletionOption.ResponseHeadersRead);

            return await response.Content.ReadFromJsonAsync<TRequest>();
        }

        public async Task<Stream> GetStreamAsync(string uri
            , AuthConfiguration authConfig = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var request = CreateRequest(HttpMethod.Get, uri, authConfig, headers: headers);
            var response = await SendAsync(request, cancellationToken, HttpCompletionOption.ResponseHeadersRead);

            return await response.Content.ReadAsStreamAsync();
        }

        #endregion

        #region POST

        public async Task<HttpResponseMessage> PostAsync(string uri
            , HttpContent requestContent
            , AuthConfiguration authConfig = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var request = CreateRequest(HttpMethod.Post, uri, authConfig, requestContent, headers: headers);
            return await SendAsync(request, cancellationToken);
        }

        public async Task<TResponse> PostAsync<TResponse>(string uri
            , HttpContent requestContent
            , AuthConfiguration authConfig = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var response = await PostAsync(uri, requestContent, authConfig, headers, cancellationToken);
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync<TRequest>(string uri
            , TRequest objectContent
            , bool ignoreNull = true
            , AuthConfiguration authConfig = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var content = CreateJsonContent(objectContent, ignoreNull);
            return await PostAsync(uri, content, authConfig, headers, cancellationToken);
        }

        public async Task<TResponse> PostAsJsonAsync<TRequest, TResponse>(string uri
            , TRequest objectContent
            , bool ignoreNull = true
            , AuthConfiguration authConfig = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var content = CreateJsonContent(objectContent, ignoreNull);
            return await PostAsync<TResponse>(uri, content, authConfig, headers, cancellationToken);
        }

        #endregion

        #region DELETE

        public async Task<HttpResponseMessage> DeleteAsync(string uri
            , AuthConfiguration authConfig = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var request = CreateRequest(HttpMethod.Delete, uri, authConfig, headers: headers);
            return await SendAsync(request, cancellationToken);
        }

        public async Task<TResponse> DeleteAsync<TResponse>(string uri
            , AuthConfiguration authConfig = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var response = await DeleteAsync(uri, authConfig, headers, cancellationToken);
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        #endregion

        #region PUT

        public async Task<HttpResponseMessage> PutAsync(string uri
            , HttpContent requestContent
            , AuthConfiguration authConfig = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var request = CreateRequest(HttpMethod.Put, uri, authConfig, content: requestContent, headers: headers);
            return await SendAsync(request, cancellationToken);
        }

        public async Task<TResponse> PutAsync<TResponse>(string uri
            , HttpContent requestContent
            , AuthConfiguration authConfig = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var response = await PutAsync(uri, requestContent, authConfig, headers, cancellationToken);
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<HttpResponseMessage> PutAsJsonAsync<TResquest>(string uri
            , TResquest objectContent
            , bool ignoreNull = true
            , AuthConfiguration authConfig = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var content = CreateJsonContent(objectContent, ignoreNull);
            return await PutAsync(uri, content, authConfig, headers, cancellationToken);
        }

        public async Task<TResponse> PutAsJsonAsync<TRequest, TResponse>(string uri
            , TRequest objectContent
            , bool ignoreNull = true
            , AuthConfiguration authConfig = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var content = CreateJsonContent(objectContent, ignoreNull);
            return await PutAsync<TResponse>(uri, content, authConfig, headers, cancellationToken);
        }

        #endregion

        #region PATCH

        public async Task<HttpResponseMessage> PatchAsync(string uri
            , HttpContent requestContent
            , AuthConfiguration authConfig = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var request = CreateRequest(HttpMethod.Patch, uri, authConfig, content: requestContent, headers: headers);
            return await SendAsync(request, cancellationToken);
        }

        public async Task<TResponse> PatchAsync<TResponse>(string uri
            , HttpContent requestContent
            , AuthConfiguration authConfig = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var response = await PatchAsync(uri, requestContent, authConfig, headers, cancellationToken);
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<HttpResponseMessage> PatchAsJsonAsync<TResquest>(string uri
            , TResquest objectContent
            , bool ignoreNull = true
            , AuthConfiguration authConfig = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var content = CreateJsonContent(objectContent, ignoreNull);
            return await PatchAsync(uri, content, authConfig, headers, cancellationToken);
        }

        public async Task<TResponse> PatchAsJsonAsync<TRequest, TResponse>(string uri
            , TRequest objectContent
            , bool ignoreNull = true
            , AuthConfiguration authConfig = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var content = CreateJsonContent(objectContent, ignoreNull);
            return await PatchAsync<TResponse>(uri, content, authConfig, headers, cancellationToken);
        }

        public JsonContent CreateJsonContent<TRequest>(TRequest objectContent, bool ignoreNull)
        {
            var jsonMediaTypeHeader = new MediaTypeHeaderValue("application/json");
            var jsonSerializerOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

            return JsonContent.Create<TRequest>(objectContent, jsonMediaTypeHeader, jsonSerializerOptions);
        }

        public StringContent CreateStringContent(object objectContent)
        {
            return new StringContent(JsonConvert.SerializeObject(objectContent), Encoding.UTF8, "application/json");
        }

        #endregion

        #region Private Functions

        private HttpRequestMessage CreateRequest(HttpMethod method
            , string uri
            , AuthConfiguration authConfig = null
            , HttpContent content = null
            , IDictionary<string, string> headers = null)
        {
            if (uri.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(uri));
            }

            var request = new HttpRequestMessage(method, uri);

            if (authConfig != null)
            {
                authConfig.Validate();

                if (authConfig.Scheme == AuthSchemes.Basic)
                {
                    request.SetBasicAuthentication(authConfig.Username, authConfig.Password);
                }
                else if (authConfig.Scheme == AuthSchemes.BearerToken)
                {
                    request.SetBearerToken(authConfig.BearerToken);
                }
            }

            if (content != null)
            {
                request.Content = content;
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            return request;
        }

        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage
            , CancellationToken? cancellationToken = null
            , HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            _logger.LogInformation("[INTERNAL Request] {Method} {Uri} content payload: {ContentLength} bytes.",
                requestMessage.Method,
                requestMessage.RequestUri.AbsoluteUri,
                requestMessage.Content?.Headers?.ContentLength
            );

            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var timer = Stopwatch.StartNew();
            var response = cancellationToken == null || cancellationToken == CancellationToken.None ?
                await _httpClient.SendAsync(requestMessage, completionOption) :
                await _httpClient.SendAsync(requestMessage, completionOption, cancellationToken.Value);
            timer.Stop();

            _logger.LogInformation("[INTERNAL Response] {Method} {Uri} in duration {Duration}ms with status code {StatusCode} and response payload: {ContentLength} bytes.",
                response?.RequestMessage?.Method,
                response?.RequestMessage?.RequestUri?.AbsoluteUri,
                timer.ElapsedMilliseconds,
                response?.StatusCode,
                response?.Content?.Headers?.ContentLength);

            return response;
        }

        #endregion
    }
}