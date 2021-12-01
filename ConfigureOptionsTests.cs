using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Config
{
    [TestClass]
    public class ConfigureOptionsTests
    {
        [TestMethod]
        public void ConfigureOptionsTest()
        {
            Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<MyHostedWorker>();
                    services.AddTransient<IConfigureOptions<MyWorkerOptionsDependsOnOtherOptions>, ConfigureMyWorkerOptions>();
                    services.AddOptions<MyBaseOptions>().Configure(_ =>
                    {
                        _.Value1 = "MyBaseOptionsValue 2";
                    });
                })
                .ConfigureAppConfiguration(configBuilder =>
                {
                    
                })
                .Build().Run();
        }
    }

    public class MyHostedWorker : BackgroundService
    {
        private IConfiguration _config;
        private IHostApplicationLifetime _lifeTime;
        IOptions<MyWorkerOptionsDependsOnOtherOptions> _myWorkerOptions;
        public MyHostedWorker(IConfiguration config, IHostApplicationLifetime lifeTime, IOptions<MyWorkerOptionsDependsOnOtherOptions> myWorkerOptions)
        {
            _config = config;
            _lifeTime = lifeTime;
            _myWorkerOptions = myWorkerOptions;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"My worker options value: {_myWorkerOptions.Value.Value1}");
            //Console.WriteLine(((IConfigurationRoot)_config).GetDebugView());
            _lifeTime.StopApplication();
            return Task.CompletedTask;
        }
    }

    public class ConfigureMyWorkerOptions : IConfigureOptions<MyWorkerOptionsDependsOnOtherOptions>
    {
        IOptions<MyBaseOptions> _baseOptions;

        public ConfigureMyWorkerOptions(IOptions<MyBaseOptions> baseOptions)
        {
            _baseOptions = baseOptions;
        }

        public void Configure(MyWorkerOptionsDependsOnOtherOptions options)
        {
            options.Value1 = _baseOptions.Value.Value1;
        }
    }

    public class MyWorkerOptionsDependsOnOtherOptions
    {
        public string? Value1 { get; set; }
    }

    public class MyBaseOptions
    {
        public string? Value1 { get; set; }
    }
}