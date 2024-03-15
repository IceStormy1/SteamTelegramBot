using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using SteamTelegramBot.Clients;
using SteamTelegramBot.Common.Extensions;
using SteamTelegramBot.Core.Extensions;
using SteamTelegramBot.Core.Profiles;
using SteamTelegramBot.Data.Extensions;
using SteamTelegramBot.Jobs.Extensions;
using SteamTelegramBot.Jobs.Jobs;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, config) => config.AddEnvironmentVariables())
    .UseSystemd()
    .ConfigureServices((context, services) =>
    {
        context.Configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json")
            .AddEnvironmentVariables()
            .Build();
        
        var httpClientTimeoutSeconds = context.Configuration.GetValue("HttpClientTimeout", defaultValue: 30);
        var httpClientTimeout = TimeSpan.FromSeconds(httpClientTimeoutSeconds);

        services
            .AddOptions()
            .AddDbContext(context.Configuration, enableSensitiveData: context.HostingEnvironment.IsDevelopment())
            .AddServices()
            .AddRepositories()
            .AddAutoMapper(x => x.AddMaps(typeof(AbstractProfile).Assembly))
            .AddClients(
                baseAddress: "http://localhost:7002",
                configuration: context.Configuration,
                clientConfigure: b => b.ConfigureHttpClient(client => client.Timeout = httpClientTimeout)
            )
            .AddQuartz(q =>
            {
                var quartzConfig = context.Configuration.GetSection("Quartz");

                q.AddJobAndTrigger<CheckingSteamAppsJob>(quartzConfig);
            })
            .AddQuartzHostedService(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            })
            ;
    }).AddSerilog();

var app = host.Build();

await app.RunAsync();