using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibQPA.Testing
{
    // https://weblog.west-wind.com/posts/2018/Feb/18/Accessing-Configuration-in-NET-Core-Test-Projects
    public class TestConfiguration
    {
        public string ConnectionString { get; set; }
        public Dictionary<string, string> Repos { get; set; }   

        public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            return new ConfigurationBuilder()
                //.SetBasePath(outputPath)
                .AddUserSecrets("7d6a17b6-7ea0-4ec5-bf97-15eaf927c0a1")
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build()
            ;
        }

        public static TestConfiguration Get(string outputPath)
        {
            var configuration = new TestConfiguration();
            var iConfig = GetIConfigurationRoot(outputPath);

            iConfig.GetSection("Configu").Bind(configuration);
            iConfig.Bind(configuration);

            return configuration;
        }
    }
}
