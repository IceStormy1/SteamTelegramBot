using System.Text.RegularExpressions;

namespace SteamTelegramBot.Common.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Escapes special characters in a given input string.
    /// </summary>
    /// <param name="text">The input string to escape special characters from.</param>
    /// <returns>The input string with special characters escaped.</returns>
    public static string ToTelegramMarkdownMessageText(this string text)
    {
        const string pattern = @"[_\*\~`>#\+\-=|{}\.!]";

        var escapedString = Regex.Replace(text, pattern, @"\$&");

        return escapedString;
    }
}