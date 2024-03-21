namespace SteamTelegramBot.Common.Constants;

/// <summary>
/// Provides message constants for Telegram bot interactions.
/// </summary>
public static class TelegramBotMessages
{
    public const string StartMessage = "Привет! 👋\n" +
                                       "Я помогу вам следить за ценами на игры в Steam и уведомлю, когда цена вашей любимой игры упадёт 🎮\n\n" +
                                       "P.S. Бот отталкивается от магазина РФ, поэтому некоторые игры могут быть недоступны";

    public const string BotDescription = "Телеграм бот поможет вам следить за ценами игр в Steam. " +
                                         "Он автоматически отслеживает изменения цен и уведомляет вас, " +
                                         "когда цена снижается, так что вы всегда можете быть в курсе выгодных предложений и сэкономить на покупках игр";

    public const string BotShortDescriptionFormat = "Телеграм бот для отслеживания цен на игры в Steam. " +
                                                    "По всем вопросам обращаться к @{0}";

    public const string NeedToApplicationName = $"Необходимо указать название игры\\. Пример: `{TelegramCommands.AddGameCommand} MyGame`";

    public const string AddGameCallbackMessage = $"Для добавления игры в список введите команду: `{TelegramCommands.AddGameCommand} MyGame`";
    public const string RemoveGameCallbackMessage = "Выберите игру из списка, которую хотите удалить";

    public const string GameNotFound = "Ничего не нашлось. Попробуй ещё раз";

    public const string RestartBot = "Перезапустить бота";
    public const string AddGameForTracking = "Добавить игру для отслеживания цены";

    public const string ApplicationWasAdded = "Игра добавлена в список";
    public const string ApplicationWasDeleted = "Игра удалена из списка";

    public const string OptionsPrompt = "Выберите один из вариантов, нажав на соответствующую кнопку";
    public const string AddGameToListPrompt = "Добавить игру в список";
    public const string RemoveGameToListPrompt = "Удалить игру из списка";
    public const string TrackedApplications = "Список отслеживаемых игр";
    public const string BackToMainMenu = "Вернуться в главное меню";
}