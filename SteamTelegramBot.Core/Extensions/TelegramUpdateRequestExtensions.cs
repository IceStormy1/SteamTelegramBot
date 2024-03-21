using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Extensions;

public static class TelegramUpdateRequestExtensions
{
    public static long? GetChatIdFromRequest(this Update update)
    {
        switch (update)
        {
            case { Message: { } message }:
                return message.Chat.Id;

            case { EditedMessage: { } message }:
                return message.Chat.Id;

            case { CallbackQuery: { } callbackQuery }:
                return callbackQuery.Message!.Chat.Id;

            case { ChosenInlineResult: { } chosenInlineResult }:
                return chosenInlineResult.From.Id;

            default:
                return null;
        }
    }
}