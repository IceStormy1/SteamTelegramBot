namespace SteamTelegramBot.Common.Constants;

public static class TelegramConstants
{
    public const string StartMessage = "Привет! 👋\n" +
                                       "Я помогу вам следить за ценами на игры в Steam и уведомлю, когда цена вашей любимой игры упадёт 🎮\n\n" +
                                       "P.S. Бот отталкивается от магазина РФ, поэтому некоторые игры могут быть недоступны";

    public const string NeedToApplicationName = $"Необходимо указать название игры\\. Пример: `{TelegramCommands.AddGameCommand} MyGame`";

    public const string AddGameCallbackMessage = $"Для добавления игры в список введите команду: `{TelegramCommands.AddGameCommand} MyGame`";
    public const string RemoveGameCallbackMessage = "Выберите игру из списка, которую хотите удалить";

    public const string GameNotFound = "Ничего не нашлось. Попробуй ещё раз";

    public const string AddAppCallback = "add_app";
    public const string RemoveAppCallback = "remove_app";
    public const string MainMenuCallback = "main_menu";
    public const string FollowedAppsCallback = "followed_apps";

}