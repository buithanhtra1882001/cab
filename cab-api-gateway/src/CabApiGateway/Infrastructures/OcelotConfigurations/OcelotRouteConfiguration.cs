using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Ocelot.Configuration.File;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CabApiGateway.Infrastructures.OcelotConfigurations
{
    public static class OcelotRouteConfiguration
    {
        public static void Setup(HostBuilderContext hostingContext, IConfigurationBuilder configuration)
        {
            const string primaryConfigFile = "ocelot.json";
            const string globalConfigFile = "ocelot.global.json";
            const string subConfigPattern = @"^ocelot\.(.*?)\.json$";

            var env = hostingContext.HostingEnvironment;

            configuration.SetBasePath(Directory.GetCurrentDirectory());
            configuration.AddJsonFile("appsettings.json");
            configuration.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);
            configuration.AddJsonFile($"appsettings.Override.json", true);
            configuration.AddEnvironmentVariables();

            var configurationGetter = configuration.Build();

            var folder = configurationGetter.GetValue<string>("OcelotRoutesFolder");
            if (string.IsNullOrEmpty(folder))
            {
                folder = "OcelotRoutes";
            }

            string excludeConfigName = env?.EnvironmentName != null ? $"ocelot.{env.EnvironmentName}.json" : string.Empty;

            var reg = new Regex(subConfigPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            var files = new DirectoryInfo(folder)
                .EnumerateFiles()
                .Where(fi => reg.IsMatch(fi.Name) && (fi.Name != excludeConfigName))
                .ToList();

            var fileConfiguration = new FileConfiguration();

            foreach (var file in files)
            {
                if (files.Count > 1 && file.Name.Equals(primaryConfigFile, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var lines = File.ReadAllText(file.FullName);

                var config = JsonConvert.DeserializeObject<FileConfiguration>(lines);

                if (file.Name.Equals(globalConfigFile, StringComparison.OrdinalIgnoreCase))
                {
                    fileConfiguration.GlobalConfiguration = config.GlobalConfiguration;
                }

                fileConfiguration.Aggregates.AddRange(config.Aggregates);
                fileConfiguration.Routes.AddRange(config.Routes);
            }

            var json = JsonConvert.SerializeObject(fileConfiguration);

            File.WriteAllText(primaryConfigFile, json);

            configuration.AddJsonFile(primaryConfigFile, false, false);
        }
    }
}