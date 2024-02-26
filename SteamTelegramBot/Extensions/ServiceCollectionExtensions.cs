using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Configurations;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace SteamTelegramBot.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection RegisterOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services
                .Configure<BotConfiguration>(configuration.GetSection(nameof(BotConfiguration)))
            ;
    }

    internal static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        return services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(type => type.ToString());
            c.CustomOperationIds(d => (d.ActionDescriptor as ControllerActionDescriptor)?.ActionName);

            c.SwaggerDoc($"v1", new OpenApiInfo
            {
                Version = $"v1",
                Title = $"{CommonConstants.ApiName} API",
            });

            const string xmlFilename = $"{CommonConstants.ApiName}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            var xmlContractDocs = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory), "*.xml");
            foreach (var fileName in xmlContractDocs) c.IncludeXmlComments(fileName);

            c.EnableAnnotations();
            c.AddEnumsWithValuesFixFilters();
        });
    }

    internal static IServiceCollection AddCorsWithDefaultPolicy(this IServiceCollection services)
    {
        return services.AddCors(options =>
        {
            options.AddDefaultPolicy(corsPolicyBuilder =>
            {
                corsPolicyBuilder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    internal static IServiceCollection AddHostedServices(this IServiceCollection services, IConfiguration configuration)
    {
        var isNeedToStartupBot = configuration
            .GetSection($"{nameof(BotConfiguration)}:{nameof(BotConfiguration.IsActive)}")
            .Get<bool>();

        if (isNeedToStartupBot)
            services.AddHostedService<ConfigureWebhook>();

        return services;
    }
}