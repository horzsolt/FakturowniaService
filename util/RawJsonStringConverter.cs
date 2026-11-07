#nullable enable

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace FakturowniaService
{
    public class RawJsonStringConverter : JsonConverter<string>
    {
        public override string? ReadJson(JsonReader reader, Type objectType, string? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.String)
                return token.Value<string>();

            return token.ToString(Formatting.None);
        }

        public override void WriteJson(JsonWriter writer, string? value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteRawValue(value);
        }
    }
}
