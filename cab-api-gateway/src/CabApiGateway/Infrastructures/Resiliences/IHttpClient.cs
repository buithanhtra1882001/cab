using System.Net.Http;
using System.Threading.Tasks;

namespace GatewayApi.Infrastructures.Resiliences
{
    public interface IHttpClient
    {
        Task<string> GetStringAsync(string uri);
        Task<T> PostAsync<T>(string uri, HttpContent data);
    }
}