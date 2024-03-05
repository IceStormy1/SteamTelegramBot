using SteamTelegramBot.Abstractions.Exceptions;
using SteamTelegramBot.Abstractions.Models;
using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Extensions;

public static class SteamAppEntityExtensions
{
    public static string ToTelegramHyperlink(this SteamAppEntity steamAppEntity, int index)
    {
        if (steamAppEntity is null)
            throw new SteamException("Steam application must not be null");

        var applicationTitle = steamAppEntity.Title;
        var formattedLink = steamAppEntity.ToFormattedApplicationLink();

        return $"{index + 1}. [{applicationTitle}]({formattedLink})";
    }

    public static TrackedAppItemDto ToTrackedAppItem(this SteamAppEntity steamAppEntity, int index)
    {
        if (steamAppEntity is null)
            throw new SteamException("Steam application must not be null");

        return new TrackedAppItemDto
        {
            Index = index + 1,
            Link = steamAppEntity.ToFormattedApplicationLink(),
            Name = steamAppEntity.Title,
            Id = steamAppEntity.Id
        };
    }

    public static string ToFormattedApplicationLink(this SteamAppEntity steamAppEntity)
        => string.Format(
            SteamConstants.ApplicationLinkFormat,
            steamAppEntity.SteamAppId);
}