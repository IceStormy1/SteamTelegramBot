using Newtonsoft.Json;
using SteamTelegramBot.Clients.Models;

namespace SteamTelegramBot.Clients.Converters;

public class ResultDataConverter : JsonConverter<ResultData>
{
    public override ResultData ReadJson(JsonReader reader, Type objectType, ResultData existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var rootObject = new ResultData();
        if (reader.TokenType != JsonToken.StartObject)
            return null;

        rootObject.Item = new App();
        while (reader.Read())
        {
            if (reader.TokenType != JsonToken.PropertyName) 
                continue;

            var propertyName = (string)reader.Value;
            switch (propertyName)
            {
                case "success":
                    reader.Read();

                    if (reader.Value != null) 
                        rootObject.Item.Success = (bool)reader.Value;

                    break;
                case "data":
                    reader.Read();
                    rootObject.Item.Data = serializer.Deserialize<AppDetail>(reader);
                    break;
            }
        }
        return rootObject;
    }

    public override void WriteJson(JsonWriter writer, ResultData value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName(nameof(ResultData.Item).ToLowerInvariant());
        serializer.Serialize(writer, value.Item);
        writer.WriteEndObject();
    }
}