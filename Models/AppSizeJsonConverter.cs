using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ReportBuilder.Models;

public class AppSizeJsonConverter : JsonConverter<double?>
{
    public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            // New format (v1.02+): numeric value like 45.0
            return reader.GetDouble();
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            // Old format (v1.01): string like "45.2 MB" or "45.2"
            var sizeString = reader.GetString();
            if (string.IsNullOrWhiteSpace(sizeString))
                return null;

            // Try to extract numeric part from string like "45.2 MB"
            var match = Regex.Match(sizeString, @"([\d.]+)");
            if (match.Success && double.TryParse(match.Groups[1].Value, out var size))
            {
                return size;
            }

            return null;
        }
        else if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            // Always write as number (v1.02 format)
            writer.WriteNumberValue(value.Value);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
