using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.Payment.WandaPay.JsonConverters;

public class WandaDecimalConverter : JsonConverter<decimal>
{
	public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> decimal.Parse(reader.GetString()!);

	public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
		=> writer.WriteRawValue(value.ToString("0.##"));
}
