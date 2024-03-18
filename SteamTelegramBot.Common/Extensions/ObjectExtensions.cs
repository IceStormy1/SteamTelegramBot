using Newtonsoft.Json;

namespace SteamTelegramBot.Common.Extensions;

public static class ObjectExtensions
{
    public static string Serialize(this object obj)
    {
        var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling  = DefaultValueHandling.Ignore };
        return JsonConvert.SerializeObject(obj ?? new object(), settings);
    }
}