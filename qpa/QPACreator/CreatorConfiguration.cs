using System;
using System.Collections.Generic;
using System.Text;

namespace QPACreator
{
    // https://weblog.west-wind.com/posts/2018/Feb/18/Accessing-Configuration-in-NET-Core-Test-Projects

    public class CreatorConfiguration
    {
        // para los puntos
        public string ConnectionStringPuntosXBus { get; set; }
        public string ConnectionStringPuntosSUBE { get; set; }

        // para convertir de empresa-interno(SUBE) a ficha de rosariobus
        public string ConnectionStringFichasXEmprIntSUBE { get; set; }

        // para los boletos
        public string ConnectionStringVentasSUBE { get; set; }

        public Dictionary<string, string> Repos { get; set; }

        //public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        //{
        //    return new ConfigurationBuilder()
        //        //.SetBasePath(outputPath)
        //        .AddUserSecrets("7d6a17b6-7ea0-4ec5-bf97-15eaf927c0a1")
        //        .AddJsonFile("appsettings.json", optional: true)
        //        .AddEnvironmentVariables()
        //        .Build()
        //    ;
        //}

        //public static TestConfiguration Get(string outputPath)
        //{
        //    var configuration = new TestConfiguration();
        //    var iConfig = GetIConfigurationRoot(outputPath);

        //    iConfig.GetSection("Configu").Bind(configuration);
        //    iConfig.Bind(configuration);

        //    return configuration;
        //}
    }
}
