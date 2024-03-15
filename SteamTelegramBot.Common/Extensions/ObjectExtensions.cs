using Newtonsoft.Json;

namespace SteamTelegramBot.Common.Extensions;

public static class ObjectExtensions
{
    public static string Serialize(this object obj)
    {
        return JsonConvert.SerializeObject(obj ?? new object());
    }
}