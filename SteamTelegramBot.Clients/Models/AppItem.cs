namespace SteamTelegramBot.Clients.Models;

public sealed class AppListResultData
{
    public AppListItem AppList { get; set; }
}

public class AppListItem
{
    public IReadOnlyCollection<AppItem> Apps { get; set; }
}

public sealed class AppItem
{
    public int AppId { get; set; }
    public string Name { get; set; }
}