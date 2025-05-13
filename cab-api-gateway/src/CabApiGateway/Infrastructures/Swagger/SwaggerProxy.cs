using CabApiGateway;
using GatewayApi.Infrastructures.Resiliences;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayApi.Infrastructures.Swagger
{
    public class SwaggerProxy : ISwaggerProxy
    {
        private const string _swaggerLatestVersion = "3.0";
        private readonly IHttpClient _httpClient;
        private readonly string _apiGateway;
        private readonly ISwaggerVersionConverter _swaggerVersionConverter;
        private readonly ILogger<SwaggerProxy> _logger;

        public SwaggerProxy(IOptions<AppSettings> options, IHttpClient httpClient, ILogger<SwaggerProxy> logger, ISwaggerVersionConverter converter)
        {
            _swaggerVersionConverter = converter;
            _logger = logger;
            _apiGateway = options.Value.GatewayUrl;
            _httpClient = httpClient;
        }

        public async Task<string> GetSwaggerJsonAsync(string swaggerEndPoint, string upStreamTemplate, string downStreamTemplate)
        {
            var jsonString = await _httpClient.GetStringAsync(swaggerEndPoint);
            var json = JsonConvert.DeserializeObject<dynamic>(jsonString);

            if (json["swagger"] != null && json["swagger"] != _swaggerLatestVersion)
            {
                var newVersion = await _swaggerVersionConverter.ConvertAsync(jsonString);
                if (newVersion != null)
                    json = newVersion;
            }

            json["servers"] = CreateApiServers();
            json["components"] = AddSecurityToComponents(json["components"]);

            var pathObject = JObject.FromObject(json["paths"]);
            var paths = pathObject.ToObject<Dictionary<string, object>>();
            var rewritePaths = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> path in paths)
            {
                var newPath = MapDownStreanToUpStream(downStreamTemplate, upStreamTemplate, path);
                if (!string.IsNullOrWhiteSpace(newPath))
                {
                    var pathValue = SetRequiredAuth(path.Value);
                    rewritePaths.Add(newPath, pathValue);
                }
            }

            json["paths"] = JObject.FromObject(rewritePaths);
            return JsonConvert.SerializeObject(json);
        }

        #region Private

        private object SetRequiredAuth(object pathMethods)
        {
            var oldPathMethods = CastObject<Dictionary<string, Dictionary<string, object>>>(pathMethods);
            var newPathMethods = new Dictionary<string, Dictionary<string, object>>();

            foreach (KeyValuePair<string, Dictionary<string, object>> pathDetail in oldPathMethods)
            {
                newPathMethods.Add(pathDetail.Key, pathDetail.Value);
                if (pathDetail.Value.ContainsKey("parameters"))
                {
                    newPathMethods[pathDetail.Key]["parameters"] = RemoveAuidParameter(pathDetail.Value["parameters"], out _);
                }
                var bearer = new List<object> { new { bearerAuth = new List<string>() } };
                newPathMethods[pathDetail.Key].Add("security", bearer);
            }
            return newPathMethods;
        }

        private List<Dictionary<string, object>> RemoveAuidParameter(object paramObject, out bool removedParam)
        {
            var paramList = CastObject<List<Dictionary<string, object>>>(paramObject);
            var paramNewList = paramList.Where(x => x["name"].ToString() != "auid" && x["name"].ToString() != "userId" && x["name"].ToString() != "userRole").ToList();
            removedParam = paramNewList.Count != paramList.Count;
            return paramNewList;
        }

        private JArray CreateApiServers()
        {
            var listServers = new List<object> { new { url = _apiGateway } };
            return JArray.FromObject(listServers);
        }

        private JObject AddSecurityToComponents(object jsonObject)
        {
            var components = new Dictionary<string, object>();
            if (jsonObject != null)
            {
                components = CastObject<Dictionary<string, object>>(jsonObject);
            }

            var securityObject = new
            {
                bearerAuth = new
                {
                    type = "http",
                    scheme = "bearer",
                    bearerFormat = "JWT"
                }
            };
            if (components.ContainsKey("securitySchemes"))
            {
                components.Remove("securitySchemes");
            }
            components.Add("securitySchemes", securityObject);
            return JObject.FromObject(components);
        }

        private string MapDownStreanToUpStream(string downTemplate, string upTemplate, KeyValuePair<string, object> path)
        {
            try
            {
                var up = upTemplate.Split("/").ToList();
                var down = downTemplate.Split("/").ToList();
                var pathBlocks = path.Key.Split("/").ToList();

                // take the parts that is dynamic in the template from downstream path
                var dynamicDownPart = new Dictionary<string, string>();
                for (int i = 0; i < down.Count; i++)
                {
                    if (IsDynamicComponent(down[i]))
                    {
                        string dynamicPart = "";
                        // in case {everything} - that mean we are reading to the end of the template, take all the rest of path as everything
                        if (down[i] == "{everything}")
                        {
                            dynamicPart = string.Join("/", pathBlocks.Skip(i).Take(pathBlocks.Count - i));
                        }
                        else
                        {
                            dynamicPart = pathBlocks[i];
                        }
                        dynamicDownPart.Add(down[i], dynamicPart);
                    }
                    else
                    {
                        // in case it's not the dynamic part, that mean it's a part of template, so it must be the same, if not, there's something go wrong
                        if (down[i] != pathBlocks[i])
                        {
                            throw new Exception("Number of component in downstream path is not equals to number of component in given downstream url template");
                        }
                    }
                }
                //Check if the upstream url template contains the dynamic parts from the downstream
                if (dynamicDownPart.Select(x => x.Key).Except(up).Any())
                {
                    throw new Exception("Dynamic parts in downstream url doesn't match the upstream given template");
                }

                // Fill all the dynamic path to upstream template to get the upstream url
                foreach (var item in dynamicDownPart)
                {
                    var index = up.IndexOf(item.Key);
                    if (index > 0)
                    {
                        up[index] = item.Value;
                    }
                }

                var newPath = string.Join("/", up);
                return newPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return string.Empty;
            }
        }

        private static T CastObject<T>(object o)
        {
            return JsonConvert.DeserializeObject<T>(o.ToString());
        }

        private static bool IsDynamicComponent(string component)
        {
            return component.StartsWith("{") && component.EndsWith("}");
        }

        #endregion Private
    }
}