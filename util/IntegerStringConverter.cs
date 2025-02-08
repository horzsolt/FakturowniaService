using System;
namespace FakturowniaService.util
{
    using Newtonsoft.Json;
    using System;
    using System.Globalization;

    public class IntegerStringConverter : JsonConverter<int?>
    {
        public override int? ReadJson(JsonReader reader, Type objectType, int? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string value = reader.Value.ToString();
                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decimalValue))
                {
                    return (int)decimalValue; // Convert decimal to int
                } else if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out int intValue))
                {
                    return intValue;
                }
            }
            else if (reader.TokenType == JsonToken.Integer)
            {
                return Convert.ToInt32(reader.Value);
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, int? value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }

}
