using Newtonsoft.Json;
using System;
using System.Globalization;

namespace FakturowniaService
{
    public class DecimalStringConverter : JsonConverter<decimal?>
    {
        public override decimal? ReadJson(JsonReader reader, Type objectType, decimal? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.Integer)
                return Convert.ToDecimal(reader.Value, CultureInfo.InvariantCulture);

            if (reader.TokenType == JsonToken.Float)
            {
                if (reader.Value is decimal decimalValue)
                    return decimalValue;

                return Convert.ToDecimal(reader.Value, CultureInfo.InvariantCulture);
            }

            if (reader.TokenType == JsonToken.String)
            {
                string value = reader.Value?.ToString();
                if (string.IsNullOrWhiteSpace(value))
                    return null;

                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                    return result;
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, decimal? value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }
    }
}
