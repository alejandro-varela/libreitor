using System;
using Serilog;
using Serilog.Events;

// lecturas interesantes
// https://www.campusmvp.es/recursos/post/como-manejar-trazas-en-net-core-con-serilog.aspx
// https://github.com/serilog/serilog/wiki/Configuration-Basics

// dotnet add package Serilog --version 2.10.0
// dotnet add package Serilog.Sinks.Console
// dotnet add package Serilog.Sinks.File

namespace PruebaSerilog
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("log.txt", LogEventLevel.Error)
                .CreateLogger()
            ;

            logger.Debug("Iniciando programa");
            logger.Debug("Hello World!");
            logger.Error("Ohh no");
            logger.Fatal("Naaa!!!");
        }
    }
}

