﻿using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Refit;
using Telegram.Bot;

namespace SteamTelegramBot.Clients;

/// <summary>
/// Региструет клиенты
/// </summary>
public static class ServiceCollectionExtensions
{
    private const string HttpClientName = "SteamTelegramBot.Clients";
    private const string TelegramClientName = "SteamTelegramBot.TelegramBotClient";

    /// <summary>
    /// Регистрирует все клиенты в контейнере зависимостей
    /// </summary>
    /// <param name="services">Контейнер зависимостей.</param>
    /// <param name="configuration"></param>
    /// <param name="baseAddress">Базовый адрес для вызовов.</param>
    /// <param name="clientConfigure"></param>
    public static IServiceCollection AddClients(
        this IServiceCollection services,
        IConfiguration configuration,
        string baseAddress,
        Action<IHttpClientBuilder> clientConfigure = null
        )

    {
        var botToken = configuration.GetSection("BotConfiguration:BotToken").Get<string>();

        return services.AddClients(new Uri(baseAddress), clientConfigure)
            .AddTelegramClient(botToken);
    }

    /// <summary>
    /// Регистрирует все клиенты в контейнере зависимостей.
    /// </summary>
    /// <param name="services">Контейнер зависимостей.</param>
    /// <param name="baseAddress">Базовый адрес для вызовов.</param>
    /// <param name="clientConfigure"></param>
    private static IServiceCollection AddClients(
        this IServiceCollection services, 
        Uri baseAddress,
        Action<IHttpClientBuilder>? clientConfigure = null
        )
    {
        var retryTimeouts = Enumerable
            .Range(1, 3)
            .Select(x => TimeSpan.FromMilliseconds(x * 500))
            .ToArray();

        var clientBuilder = services
            .AddHttpClient(HttpClientName, config => { config.BaseAddress = baseAddress; })
            .AddTransientHttpErrorPolicy(p => p.Or<SocketException>().WaitAndRetryAsync(retryTimeouts));

        clientConfigure?.Invoke(clientBuilder);

        services.AddRestClient<ISteamWebApiClient>();
        services.AddRestClient<IStoreSteamPoweredClient>();

        return services;
    }

    private static void AddRestClient<T>(this IServiceCollection services) where T : class
    {
        services.AddSingleton(_ => RequestBuilder.ForType<T>());
        services.AddTransient(p =>
        {
            var httpClient = p.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientName);

            if (typeof(T) == typeof(ISteamWebApiClient))
            {
                httpClient.BaseAddress = new Uri("https://api.steampowered.com/");
            }
            else if (typeof(T) == typeof(IStoreSteamPoweredClient))
            {
                httpClient.BaseAddress = new Uri("https://store.steampowered.com");
            }

            var refitSettings = new RefitSettings(new NewtonsoftJsonContentSerializer());
            var requestBuilder = RequestBuilder.ForType<T>(refitSettings);

            return RestService.For(httpClient, requestBuilder);
        });
    }

    private static IServiceCollection AddTelegramClient(this IServiceCollection services, string botToken)
    {
        services.AddHttpClient(TelegramClientName)
            .AddTypedClient<ITelegramBotClient>(httpClient =>
            {
                TelegramBotClientOptions options = new(botToken);
                return new TelegramBotClient(options, httpClient);
            });

        return services;
    }
}