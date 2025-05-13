using CabUserService.Clients.Base;
using CabUserService.Clients.Interfaces;
using CabUserService.Infrastructures.Communications.Http;
using CabUserService.Infrastructures.Extensions;
using CabUserService.Models.Dtos;
using System.Net.Http.Headers;

namespace CabUserService.Clients
{
    public class MediaClient : BaseClient<MediaClient>, IMediaClient
    {
        private readonly string _baseUrl;

        public MediaClient(IHttpClientWrapper httpClient
            , IConfiguration configuration)
        : base(httpClient)
        {
            _baseUrl = configuration.GetValue<string>("GlobalService:BaseAddress");
        }

        public async Task<IEnumerable<ImageUploadResponse>> UploadImagesAsync(string bearerToken, string type, IFormFileCollection files)
        {
            var uri = new Uri($"{_baseUrl}/v1/media-service/images/upload");

            try
            {
                using (var multipartContent = new MultipartFormDataContent())
                {
                    multipartContent.Add(new StringContent(type), nameof(type));

                    foreach (var file in files)
                    {
                        var content = new ByteArrayContent(await file.GetBytesAsync());
                        content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                        multipartContent.Add(content, "files", file.FileName);
                    }

                    var httpResponse = await _httpClient.PostAsync(uri.AbsoluteUri, multipartContent, new AuthConfiguration(bearerToken));

                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        var response = await httpResponse.Content.ReadAsStringAsync();
                        throw new HttpClientWrapperException(httpResponse.StatusCode, uri, new Exception(response));
                    }

                    return await httpResponse.Content.ReadFromJsonAsync<IEnumerable<ImageUploadResponse>>();
                }
            }
            catch (HttpClientWrapperException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new HttpClientWrapperException(System.Net.HttpStatusCode.InternalServerError, uri, ex);
            }
        }

        public async Task<IEnumerable<ImageUploadResponse>> UploadImageAsync(string bearerToken, string type, IFormFile file)
        {
            var uri = new Uri($"{_baseUrl}/v1/media-service/images/upload");

            try
            {
                using var multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new StringContent(type), nameof(type));

                var content = new ByteArrayContent(await file.GetBytesAsync());
                content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                multipartContent.Add(content, "files", file.FileName);

                var httpResponse = await _httpClient.PostAsync(uri.AbsoluteUri, multipartContent, new AuthConfiguration(bearerToken));

                if (!httpResponse.IsSuccessStatusCode)
                {
                    var response = await httpResponse.Content.ReadAsStringAsync();
                    throw new HttpClientWrapperException(httpResponse.StatusCode, uri, new Exception(response));
                }

                return await httpResponse.Content.ReadFromJsonAsync<IEnumerable<ImageUploadResponse>>();
            }
            catch (HttpClientWrapperException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new HttpClientWrapperException(System.Net.HttpStatusCode.InternalServerError, uri, ex);
            }
        }
    }
}
