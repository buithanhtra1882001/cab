using GatewayApi.Infrastructures.Resiliences;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GatewayApi.Infrastructures.Swagger
{
    public class SwaggerVersionConverter : ISwaggerVersionConverter
    {
        private readonly IHttpClient _httpClient;
        private readonly ILogger<SwaggerVersionConverter> _logger;
        private readonly string _swaggerConverterServiceEndpoint;

        public SwaggerVersionConverter(IHttpClient httpClient, ILogger<SwaggerVersionConverter> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _swaggerConverterServiceEndpoint = "https://converter.swagger.io/api/convert";
        }

        public async Task<dynamic> ConvertAsync(string jsonString)
        {
            try
            {
                var data = new StringContent(jsonString, Encoding.UTF8, "application/json");
                return await _httpClient.PostAsync<dynamic>(_swaggerConverterServiceEndpoint, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}