using Newtonsoft.Json;
using System;
using System.Globalization;

namespace FakturowniaService
{
    public class PaymentPaidConverter : JsonConverter<decimal?>
    {
        public override decimal? ReadJson(JsonReader reader, Type objectType, decimal? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.Boolean)
                return reader.Value is bool paid && paid ? 1m : 0m;

            if (reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.Float)
                return Convert.ToDecimal(reader.Value, CultureInfo.InvariantCulture);

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
            writer.WriteValue(value);
        }
    }
}
