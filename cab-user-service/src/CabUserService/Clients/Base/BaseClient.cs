using CabUserService.Infrastructures.Communications.Http;

namespace CabUserService.Clients.Base
{
    public abstract class BaseClient<T>
    {
        protected readonly IHttpClientWrapper _httpClient;

        protected BaseClient(IHttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }
    }
}