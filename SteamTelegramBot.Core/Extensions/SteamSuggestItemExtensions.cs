using SteamTelegramBot.Abstractions.Models;

namespace SteamTelegramBot.Core.Extensions;

public static class SteamSuggestItemExtensions
{
    public static string ToTelegramHyperlink(this SteamSuggestItem steamSuggestItem, int index)
        => $"{index + 1}. [{steamSuggestItem.Name}]({steamSuggestItem.StoreLink})";
}