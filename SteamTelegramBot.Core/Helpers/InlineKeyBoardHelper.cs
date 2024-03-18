using SteamTelegramBot.Abstractions;
using SteamTelegramBot.Abstractions.Models.Applications;
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
            .Append(GetMainMenuButton())
            ;

        return new InlineKeyboardMarkup(applicationButtons);
    }

    private static InlineKeyboardButton[] GetMainMenuButton()
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