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

//https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0#use-managed-identities-for-azure-resources
namespace Config.KeyVaultTest
{

    [TestClass]
    public class KeyVaultTests
    {
        [TestMethod]
        public void KeyVaultTest()
        {
            Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<TestHostedWorker>();
                })
                .ConfigureAppConfiguration(configBuilder =>
                {
                    var secretClient = new SecretClient(
                        new Uri($"https://omahadotnet.vault.azure.net/"),
                        new DefaultAzureCredential());
                    configBuilder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
                })
                .ConfigureLogging((hostBuilderContext, logBuilder) =>
                {
                    logBuilder.ClearProviders();
                })
                .Build().Run();
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
            Console.WriteLine($"My worker key vault value: '{_config["test"]}'");
            _lifeTime.StopApplication();
            return Task.CompletedTask;
        }
    }
}