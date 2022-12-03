using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MastodonBot.Interfaces;
using MastodonBot.Services;
using MastodonBot.Workers;

public class Program
{
    public static async Task Main(string[] args)
    {
        var hostBuilder = CreateDefaultBuilder();
        var host = hostBuilder.Build();

        await host.RunAsync();
    }

    private static IHostBuilder CreateDefaultBuilder()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(app => 
            {
                app
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile("local.appsettings.json", true, true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) => 
            {
                services.AddSingleton<IConfigurationService, ConfigurationService>();
                services.AddSingleton<IRegistrationService, RegistrationService>();
                services.AddSingleton<IResponseService, ResponseService>();
                services.AddSingleton<IAccountService, AccountService>();
                services.AddSingleton<ITootService, TootService>();
                services.AddHostedService<Bot>();
            });

        return builder;
    }
}
