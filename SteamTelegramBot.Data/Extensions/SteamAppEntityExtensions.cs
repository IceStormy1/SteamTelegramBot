using SteamTelegramBot.Abstractions.Exceptions;
using SteamTelegramBot.Abstractions.Models.Applications;
using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Extensions;

/// <summary>
/// Provides extension methods for <see cref="SteamAppEntity"/> objects.
/// </summary>
public static class SteamAppEntityExtensions
{
    /// <summary>
    /// Converts a Steam application entity to a formatted hyperlink suitable for Telegram.
    /// </summary>
    /// <param name="steamAppEntity">The Steam application entity.</param>
    /// <param name="index">The index of the application.</param>
    /// <returns>A formatted hyperlink for Telegram.</returns>
    public static string ToTelegramHyperlink(this SteamAppEntity steamAppEntity, int index)
    {
        if (steamAppEntity is null)
            throw new SteamException("Steam application must not be null");

        var applicationTitle = steamAppEntity.Title;
        var formattedLink = steamAppEntity.ToFormattedApplicationLink();

        return $"{index + 1}. [{applicationTitle}]({formattedLink})";
    }

    /// <summary>
    /// Converts a Steam application entity to a DTO representing a tracked application item.
    /// </summary>
    /// <param name="steamAppEntity">The Steam application entity.</param>
    /// <param name="index">The index of the application.</param>
    /// <returns>A DTO representing the tracked application item.</returns>
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

    /// <summary>
    /// Formats the link to the Steam application.
    /// </summary>
    /// <param name="steamAppEntity">The Steam application entity.</param>
    /// <returns>The formatted link to the Steam application.</returns>
    public static string ToFormattedApplicationLink(this SteamAppEntity steamAppEntity)
        => string.Format(
            SteamConstants.ApplicationLinkFormat,
            steamAppEntity.SteamAppId);
}