using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

namespace Config.AppConfig
{

    [TestClass]
    public class AppConfigTests
    {
        [TestMethod]
        public void AppConfigTest()
        {
            Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<TestHostedWorker>();
                })
                .ConfigureAppConfiguration(configBuilder =>
                {
                    configBuilder.AddAzureAppConfiguration("Endpoint=https://omahadotnet.azconfig.io;Id=46g3-l5-s0:kBqdF2CfaI5jH/I/oKJz;Secret=Ih6SnPdCtCT//+QRbtJFmTnp5EN9xZ6Vep0F54wwowk=");
                })
                .ConfigureLogging((hostBuilderContext, logBuilder) =>
                {
                    logBuilder.ClearProviders();
                }).Build().Run();
        }
    }

    public class TestHostedWorker : BackgroundService
    {
        private IConfiguration _config;
        private IHostApplicationLifetime _lifeTime;

        public TestHostedWorker(IConfiguration config, IHostApplicationLifetime lifeTime)
        {
            _config = config;
            _lifeTime = lifeTime;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"My worker key vault value: {_config["test"]}");
            _lifeTime.StopApplication();
            return Task.CompletedTask;
        }
    }
}