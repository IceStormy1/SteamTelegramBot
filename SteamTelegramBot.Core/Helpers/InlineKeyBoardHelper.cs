using SteamTelegramBot.Abstractions;
using SteamTelegramBot.Abstractions.Models.Applications;
using SteamTelegramBot.Abstractions.Models.Callbacks;
using SteamTelegramBot.Common.Enums;
using SteamTelegramBot.Common.Extensions;
using Telegram.Bot.Types.ReplyMarkups;

namespace SteamTelegramBot.Core.Helpers;

/// <summary>
/// Provides helper methods for creating inline keyboards for Telegram bot interactions.
/// </summary>
public static class InlineKeyBoardHelper
{
    /// <summary>
    /// Gets an inline keyboard markup based on the specified inline keyboard type.
    /// </summary>
    /// <param name="type">The type of inline keyboard.</param>
    /// <returns>An inline keyboard markup.</returns>
    public static InlineKeyboardMarkup GetInlineKeyboardByType(InlineKeyBoardType type)
    {
        var inlineKeyBoardButtons = type switch
        {
            InlineKeyBoardType.Start => GetMainMenuButtons(),
            InlineKeyBoardType.BackToMainMenu or InlineKeyBoardType.AddGame => new List<IEnumerable<InlineKeyboardButton>> { GetBackMainMenuButton() },
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        return new InlineKeyboardMarkup(inlineKeyBoardButtons);
    }

    /// <summary>
    /// Gets an inline keyboard markup for adding an application, based on Steam suggest items.
    /// </summary>
    /// <param name="steamSuggestItems">The collection of Steam suggest items.</param>
    /// <returns>An inline keyboard markup for adding a game.</returns>
    public static InlineKeyboardMarkup GetAddGameInlineKeyboard(IReadOnlyCollection<SteamSuggestItem> steamSuggestItems)
    {
        var keyboardButtons = steamSuggestItems.Select(x => new[]
        {
            InlineKeyboardButton.WithCallbackData(
                text: x.Name,
                callbackData: new ChosenAppCallbackDto { Action = AppAction.Add, AppId = x.AppId }.Serialize()),
        });

        keyboardButtons = keyboardButtons.Append(GetBackMainMenuButton());

        return new InlineKeyboardMarkup(keyboardButtons);
    }

    /// <summary>
    /// Gets an inline keyboard markup for paging by application action.
    /// </summary>
    /// <param name="trackedApps">The collection of tracked application items.</param>
    /// <param name="pageInfo">The paging information.</param>
    /// <param name="appAction">The application action.</param>
    /// <returns>An inline keyboard markup for paging by application action.</returns>
    public static InlineKeyboardMarkup GetPagedInlineKeyboardByAppAction(ICollection<TrackedAppItemDto> trackedApps, IPaged pageInfo, AppAction appAction)
    {
        var applicationButtons = trackedApps.Select(x => new[]
        {
            appAction == AppAction.Remove
                ? InlineKeyboardButton.WithCallbackData(
                    text: x.FormattedTitle,
                    callbackData: new ChosenAppCallbackDto { Action = appAction, AppId = x.Id, Current = pageInfo.Current}.Serialize())
                : InlineKeyboardButton.WithUrl(text: x.FormattedTitle, url: x.Link),
        })
            .Append(GetPagingButtons(pageInfo, appAction))
            .Append(GetBackMainMenuButton())
            ;

        return new InlineKeyboardMarkup(applicationButtons);
    }

    private static List<IEnumerable<InlineKeyboardButton>> GetMainMenuButtons()
        => new()
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Добавить игру в список",
                    new AddAppCallbackDto().Serialize()),
                InlineKeyboardButton.WithCallbackData("Удалить игру из списка",
                    new RemoveAppCallbackDto().Serialize()),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Список отслеживаемых игр",
                    new TrackedAppsCallbackDto().Serialize())
            }
        };

    private static InlineKeyboardButton[] GetBackMainMenuButton()
        => new[]
        {
            InlineKeyboardButton.WithCallbackData("Вернуться в главное меню", new MainMenuCallbackDto().Serialize())
        };

    private static InlineKeyboardButton[] GetPagingButtons(IPaged pageInfo, AppAction appAction)
    {
        var callback = GetCallbackModelForAction(pageInfo, appAction);

        var isFirstPage = pageInfo.Current == IPaged.DefaultPage;
        var isLastPage = pageInfo.Current == pageInfo.Total;

        var previousButtonText = isFirstPage ? "⛔️" : "⬅️";
        callback.Current = isFirstPage ? pageInfo.Current : pageInfo.Current - 1;
        callback.Ignore = isFirstPage;
        var previousButton = InlineKeyboardButton.WithCallbackData(text: previousButtonText, callbackData: callback.Serialize());

        var nextButtonText = isLastPage ? "⛔️" : "➡️";
        callback.Current = isLastPage ? pageInfo.Current : pageInfo.Current + 1;
        callback.Ignore = isLastPage;
        var nextButton = InlineKeyboardButton.WithCallbackData(text: nextButtonText, callbackData: callback.Serialize());

        var pageCountText = $"{pageInfo.Current} / {pageInfo.Total}";
        callback.Current = pageInfo.Current;
        callback.Ignore = true;
        var pageCountButton = InlineKeyboardButton.WithCallbackData(text: pageCountText, callbackData: callback.Serialize());

        return new[] { previousButton, pageCountButton, nextButton };
    }

    private static PagedCallbackDto GetCallbackModelForAction(IPaged pageInfo, AppAction appAction)
    {
        var result = new PagedCallbackDto();

        switch (appAction)
        {
            case AppAction.Remove:
                result = new RemoveAppCallbackDto();
                break;

            case AppAction.Add:
            case AppAction.Get:
                result = new TrackedAppsCallbackDto();
                break;
        }

        result.Total = pageInfo.Total;
        
        return result;
    }
}