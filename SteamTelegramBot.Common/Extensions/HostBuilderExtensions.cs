﻿using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;

namespace SteamTelegramBot.Common.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddSerilog(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((ctx, _, cfg) =>
            cfg
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder())
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .ReadFrom.Configuration(ctx.Configuration)
                .MinimumLevel.Override("System", LogEventLevel.Information)
        );
    }
}