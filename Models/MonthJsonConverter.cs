using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReportBuilder.Models;

public class MonthJsonConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            // New format (v1.02+): numeric month
            return reader.GetInt32();
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            // Old format (v1.01): month name like "October"
            var monthName = reader.GetString();
            if (string.IsNullOrWhiteSpace(monthName))
                return 0;

            try
            {
                var date = DateTime.ParseExact(monthName, "MMMM", System.Globalization.CultureInfo.InvariantCulture);
                return date.Month;
            }
            catch
            {
                // If parsing fails, return 0
                return 0;
            }
        }

        return 0;
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        // Always write as number (v1.02 format)
        writer.WriteNumberValue(value);
    }
}
