using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakturowniaService
{
    public class DecimalStringConverter : JsonConverter<decimal?>
    {
        public override decimal? ReadJson(JsonReader reader, Type objectType, decimal? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string value = reader.Value.ToString();
                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    return result;
                }
            }
            return null; // Return null if not a valid decimal string
        }

        public override void WriteJson(JsonWriter writer, decimal? value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }
    }
}
