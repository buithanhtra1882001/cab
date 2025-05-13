using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace CabUserService.Tests.TestHelper;
public class MockObjects
{
    public static IConfiguration IConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"UserDbConnectionString", "Host=localhost;Database=UserDb;Username=postgres;Password=mysecretpassword"},
            //...populate as needed for the test
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        return configuration;
    }
}
