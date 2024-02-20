using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using SteamTelegramBot.Clients;
using SteamTelegramBot.Data.Extensions;
using SteamTelegramBot.Data.Helpers;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

const string apiName = "SteamTelegramBot";

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, _, cfg) =>
    cfg
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder())
        .Enrich.WithThreadId()
        .Enrich.WithThreadName()
        .ReadFrom.Configuration(ctx.Configuration)
        .MinimumLevel.Override("System", LogEventLevel.Information)
);

builder.Services
    .AddDbContext(builder.Configuration, enableSensitiveData: builder.Environment.IsDevelopment())
    .AddRouting(c => c.LowercaseUrls = true)
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
    .AddSwaggerGen(c =>
    {
        c.CustomSchemaIds(type => type.ToString());
        c.CustomOperationIds(d => (d.ActionDescriptor as ControllerActionDescriptor)?.ActionName);

        c.SwaggerDoc($"v1", new OpenApiInfo
        {
            Version = $"v1",
            Title = $"{apiName} API",
        });

        const string xmlFilename = $"{apiName}.xml";
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

        var xmlContractDocs = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory), "*.xml");
        foreach (var fileName in xmlContractDocs) c.IncludeXmlComments(fileName);

        c.EnableAnnotations();
        c.AddEnumsWithValuesFixFilters();
    })
    .AddClients(
        baseAddress: "http://localhost:7002", 
        clientConfigure: b => b.ConfigureHttpClient(client => client.Timeout = httpClientTimeout)
        );

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsPolicyBuilder =>
    {
        corsPolicyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c => { c.SerializeAsV2 = true; })
        .UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{apiName} API V1");
            c.RoutePrefix = string.Empty;
            c.DocumentTitle = $"{apiName} Documentation";
            c.DocExpansion(DocExpansion.None);
        });
}

app
    .UseStatusCodePages()
    .UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();

MigrationTool.Execute(app.Services);

app.Run();
