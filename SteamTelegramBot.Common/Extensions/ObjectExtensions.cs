using Newtonsoft.Json;

namespace SteamTelegramBot.Common.Extensions;

/// <summary>
/// Provides extension methods for object manipulation and serialization.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Serializes an object to a JSON string, ignoring null and default property values.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>A JSON string representing the serialized object.</returns>
    public static string Serialize(this object obj)
    {
        var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling  = DefaultValueHandling.Ignore };
        return JsonConvert.SerializeObject(obj ?? new object(), settings);
    }
}