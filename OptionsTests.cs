using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Config.Options
{

    [TestClass]
    public class OptionsTests
    {
        [TestMethod]
        public void OptionsTest()
        {
            Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<TestHostedWorker>();
                    services.Configure<MyOptions1>(myOptions =>
                    {
                        myOptions.Value1 = "Value1";
                    });
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
        IOptions<MyOptions1> _myOptions;
        public TestHostedWorker(IConfiguration config, IHostApplicationLifetime lifeTime, IOptions<MyOptions1> myOptions)
        {
            _config = config;
            _lifeTime = lifeTime;
            _myOptions = myOptions;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"My worker options value: {_myOptions.Value.Value1}");
            //Console.WriteLine(((IConfigurationRoot)_config).GetDebugView());
            _lifeTime.StopApplication();
            return Task.CompletedTask;
        }
    }

    public class MyOptions1
    {
        public string Value1 { get; set; }
        public string Value2 { get; set; }
    }
}