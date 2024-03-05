using SteamTelegramBot.Abstractions.Models;
using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Common.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SteamTelegramBot.Core.Helpers;

public static class InlineKeyBoardHelper
{
    public static InlineKeyboardMarkup GetInlineKeyboardByType(InlineKeyBoardType type)
    {
        List<IEnumerable<InlineKeyboardButton>> inlineKeyBoardButtons;
        switch (type)
        {
            case InlineKeyBoardType.Start:
                inlineKeyBoardButtons = new List<IEnumerable<InlineKeyboardButton>>
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Добавить игру в список", TelegramConstants.AddAppCallback),
                        InlineKeyboardButton.WithCallbackData("Удалить игру из списка", TelegramConstants.RemoveAppCallback),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Список отслеживаемых игр", TelegramConstants.FollowedAppsCallback),
                    }
                };

                return new InlineKeyboardMarkup(inlineKeyBoardButtons);

            case InlineKeyBoardType.AddGame:
                inlineKeyBoardButtons = new List<IEnumerable<InlineKeyboardButton>>
                {
                    GetMainMenuButton()
                };

                return new InlineKeyboardMarkup(inlineKeyBoardButtons);
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public static InlineKeyboardMarkup GetAddGameInlineKeyboard(IReadOnlyCollection<SteamSuggestItem> steamSuggestItems)
    {
        var keyboardButtons = steamSuggestItems.Select(x => new[]
        {
            InlineKeyboardButton.WithCallbackData(text: x.Name, callbackData: $"{AppAction.Add} {x.AppId}"),
        });

        keyboardButtons = keyboardButtons.Append(GetMainMenuButton());

        return new InlineKeyboardMarkup(keyboardButtons);
    }

    public static InlineKeyboardMarkup GetInlineKeyboardByAppAction(ICollection<TrackedAppItemDto> trackedApps, AppAction appAction)
    {
        var keyboardButtons = trackedApps.Select(x => new[]
        {
            appAction == AppAction.Remove
                ? InlineKeyboardButton.WithCallbackData(text: x.Name, callbackData: $"{AppAction.Remove} {x.Id}")
                : InlineKeyboardButton.WithUrl(text: $"{x.Index}. {x.Name}", url: x.Link),
        });

        keyboardButtons = keyboardButtons.Append(GetMainMenuButton());

        return new InlineKeyboardMarkup(keyboardButtons);
    }

    private static InlineKeyboardButton[] GetMainMenuButton()
        => new[]
        {
            InlineKeyboardButton.WithCallbackData("Вернуться в главное меню", TelegramConstants.MainMenuCallback)
        };
}