using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SteamTelegramBot.Abstractions.Exceptions;
using SteamTelegramBot.Clients;
using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Core.Extensions;
using SteamTelegramBot.Core.Profiles;
using SteamTelegramBot.Data.Extensions;
using SteamTelegramBot.Data.Helpers;
using SteamTelegramBot.Extensions;
using Swashbuckle.AspNetCore.SwaggerUI;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilog();

builder.Services
    .RegisterOptions(builder.Configuration)
    .AddDbContext(builder.Configuration, enableSensitiveData: builder.Environment.IsDevelopment())
    .AddServices()
    .AddRouting(c => c.LowercaseUrls = true)
    .AddAutoMapper(x => x.AddMaps(typeof(AbstractProfile).Assembly))
    .AddHostedServices(builder.Configuration)
    ;

builder.Services.AddControllers()
    .AddNewtonsoftJson(cfg =>
    {
        cfg.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        cfg.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        cfg.SerializerSettings.Converters.Add(new StringEnumConverter());
    });

var httpClientTimeoutSeconds = builder.Configuration.GetValue("HttpClientTimeout", defaultValue: 30);
var httpClientTimeout = TimeSpan.FromSeconds(httpClientTimeoutSeconds);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwagger()
    .AddClients(
        baseAddress: "http://localhost:7002", 
        configuration:builder.Configuration,
        clientConfigure: b => b.ConfigureHttpClient(client => client.Timeout = httpClientTimeout)
        )
    .AddCorsWithDefaultPolicy();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c => { c.SerializeAsV2 = true; })
        .UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{CommonConstants.ApiName} API V1");
            c.RoutePrefix = string.Empty;
            c.DocumentTitle = $"{CommonConstants.ApiName} Documentation";
            c.DocExpansion(DocExpansion.None);
        });
}

app
    .UseStatusCodePages()
    .UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization()
    .UseExceptionHandler(applicationBuilder => applicationBuilder.Run(context => HandleError(context, applicationBuilder.ApplicationServices)));

app.MapControllers();

MigrationTool.Execute(app.Services);

app.Run();


static async Task HandleError(HttpContext context, IServiceProvider serviceProvider)
{
    var logger = serviceProvider.GetRequiredService<ILogger<HttpContext>>();
    var telegramClient = serviceProvider.GetRequiredService<ITelegramBotClient>();

    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
    var exception = exceptionHandlerPathFeature?.Error;

    object logMessage = new
    {
        Message = AbstractConstants.InternalError,
        Path = context.Request.Path.ToString(),
        context.Request.Method
    };

    logger.LogError(exception, "{@Response}", logMessage);

    if (exception is TelegramException telegramException)
    {
        context.Response.StatusCode = StatusCodes.Status200OK; // Telegram will do a retry in case of an error, so the status is 200
        await telegramClient.SendTextMessageAsync(
            chatId: telegramException.ChatId,
            text: exception.Message);
    }
}