using AutoMapper;
using CabPostService.Infrastructures.Communications.Http;

namespace CabPostService.Clients.Base
{
    public abstract class BaseClient<T>
    {
        protected readonly IHttpClientWrapper _httpClient;
        protected readonly IMapper _mapper;

        protected BaseClient(
            IHttpClientWrapper httpClient,
            IMapper mapper)
        {
            _httpClient = httpClient;
            _mapper = mapper;
        }
    }
}