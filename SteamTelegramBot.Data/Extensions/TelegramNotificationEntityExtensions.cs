using SteamTelegramBot.Abstractions.Exceptions;
using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Extensions;

public static class TelegramNotificationEntityExtensions
{
    public static string ToTelegramHyperlink(this TelegramNotificationEntity telegramNotificationEntity, int index)
    {
        if (telegramNotificationEntity.SteamAppPrice?.SteamApp is null)
            throw new SteamException("Steam application must not be null");

        var applicationTitle = telegramNotificationEntity.SteamAppPrice.SteamApp.Title;
        var formattedLink = string.Format(SteamConstants.ApplicationLinkFormat,
            telegramNotificationEntity.SteamAppPrice.SteamApp.SteamAppId);

        return $"{index + 1}. [{applicationTitle}]({formattedLink})";
    }
}