using System.Threading.Tasks;

namespace GatewayApi.Infrastructures.Swagger
{
    public interface ISwaggerProxy
    {
        Task<string> GetSwaggerJsonAsync(string swaggerEndPoint, string upStreamTemplate, string downStreamTemplate);
    }
}