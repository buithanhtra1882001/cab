using AutoMapper;
using CabPostService.Clients.Base;
using CabPostService.Clients.Interfaces;
using CabPostService.Infrastructures.Communications.Http;
using CabPostService.Infrastructures.Extensions;
using CabPostService.Models.Dtos;
using System.Net.Http.Headers;

namespace CabPostService.Clients
{
    public class MediaClient :
        BaseClient<MediaClient>,
        IMediaClient
    {
        private readonly string _baseUrl;

        public MediaClient(
            IHttpClientWrapper httpClient,
            IConfiguration configuration,
            IMapper mapper)
        : base(httpClient, mapper)
        {
            _baseUrl = configuration.GetValue<string>("MediaService:BaseAddress");
        }

        public async Task UpdateImagesAsync(string bearerToken, IEnumerable<Guid> guids)
        {
            var uri = new Uri($"{_baseUrl}/v1/media-service/images/update");

            try
            {
                var requestContent = _httpClient.CreateStringContent(new
                {
                    guids,
                    lastUsedAt = DateTime.UtcNow
                });
                var httpResponse = await _httpClient.PutAsync(
                    uri.AbsoluteUri,
                    requestContent,
                    new AuthConfiguration(bearerToken));

                if (!httpResponse.IsSuccessStatusCode)
                {
                    var response = await httpResponse.Content.ReadAsStringAsync();
                    throw new HttpClientWrapperException(httpResponse.StatusCode, uri, new Exception(response));
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

        public async Task<IEnumerable<VideoUploadResponse>?> UploadVideoAsync(
            string bearerToken,
            string type,
            IFormFileCollection files)
        {
            var uri = new Uri($"{_baseUrl}/v1/media-service/video/upload");

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

                    return await httpResponse.Content.ReadFromJsonAsync<IEnumerable<VideoUploadResponse>>();
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
    }
}
