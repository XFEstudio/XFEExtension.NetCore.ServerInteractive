using System.Text.Json;
using System.Text.Json.Serialization;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;

/// <summary>
/// JsonDateTime转换器
/// </summary>
public class JsonDateTimeConverter : JsonConverter<DateTime>
{

    /// <inheritdoc/>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        if(dateString is null)
            return DateTime.MinValue;
        return DateTime.Parse(dateString);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
}
