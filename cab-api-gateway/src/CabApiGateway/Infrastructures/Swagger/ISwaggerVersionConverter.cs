using System.Threading.Tasks;

namespace GatewayApi.Infrastructures.Swagger
{
    public interface ISwaggerVersionConverter
    {
        Task<dynamic> ConvertAsync(string jsonString);
    }
}