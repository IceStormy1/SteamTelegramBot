using SteamTelegramBot.Abstractions.Models;
using SteamTelegramBot.Abstractions.Models.Callbacks;
using SteamTelegramBot.Common.Enums;
using SteamTelegramBot.Common.Extensions;
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
                        InlineKeyboardButton.WithCallbackData("Добавить игру в список", new AddAppCallbackDto().Serialize()),
                        InlineKeyboardButton.WithCallbackData("Удалить игру из списка", new RemoveAppCallbackDto().Serialize()),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Список отслеживаемых игр", new TrackedAppsCallbackDto().Serialize())
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
            InlineKeyboardButton.WithCallbackData(
                text: x.Name,
                callbackData: new ChosenAppCallbackDto { Action = AppAction.Add, AppId = x.AppId }.Serialize()),
        });

        keyboardButtons = keyboardButtons.Append(GetMainMenuButton());

        return new InlineKeyboardMarkup(keyboardButtons);
    }

    public static InlineKeyboardMarkup GetInlineKeyboardByAppAction(ICollection<TrackedAppItemDto> trackedApps, AppAction appAction)
    {
        var keyboardButtons = trackedApps.Select(x => new[]
        {
            appAction == AppAction.Remove
                ? InlineKeyboardButton.WithCallbackData(
                    text: x.Name,
                    callbackData: new ChosenAppCallbackDto { Action = AppAction.Remove, AppId = x.Id }.Serialize())
                : InlineKeyboardButton.WithUrl(text: $"{x.Index}. {x.Name}", url: x.Link),
        });

        keyboardButtons = keyboardButtons.Append(GetMainMenuButton());

        return new InlineKeyboardMarkup(keyboardButtons);
    }

    private static InlineKeyboardButton[] GetMainMenuButton()
        => new[]
        {
            InlineKeyboardButton.WithCallbackData("Вернуться в главное меню", new MainMenuCallbackDto().Serialize())
        };
}