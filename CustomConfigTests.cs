using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Config.CustomConfig
{
    [TestClass]
    public class CustomConfigTests
    {
        [TestMethod]
        public void CustomConfigTest()
        {
            Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<TestHostedWorker>();
                })
                .ConfigureAppConfiguration(builder =>
                {
                    builder.Sources.Clear();
                    builder.Sources.Add(new MyCustomSource());
                    System.Console.WriteLine(builder.Sources.Count);
                })
                .ConfigureLogging((hostBuilderContext, logBuilder) =>
                {
                    logBuilder.ClearProviders();
                }).Build().Run();
        }
    }

    public class TestHostedWorker : BackgroundService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly IConfiguration _configuration;

        public TestHostedWorker(IHostApplicationLifetime applicationLifetime, IConfiguration configuration)
        {
            _applicationLifetime = applicationLifetime;
            _configuration = configuration;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            System.Console.WriteLine($"MyValue1 = \"{_configuration["MyValue1"]}\"");
            System.Console.WriteLine($"MyValue2 = \"{_configuration["MyValue2"]}\"");
            _applicationLifetime.StopApplication();
            return Task.CompletedTask;
        }
    }

    public class MyCustomSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder) => new MyCustomProvider();
    }

    public class MyCustomProvider : ConfigurationProvider
    {
        public override bool TryGet(string key, out string value)
        {
            value = string.Empty;
            switch (key)
            {
                case "MyValue1":
                    value = "MyValue1";
                    return true;
            }

            return false;
        }
    }
}