using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.Payment.WandaPay.JsonConverters;

public class WandaDateTimeConverter : JsonConverter<DateTime>
{
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var unspecifyDate = DateTime.ParseExact(reader.GetString()!, "yyyyMMddHHmmss", CultureInfo.CurrentCulture);

		var specifyDate = new DateTimeOffset(
			unspecifyDate,
			TimeSpan.FromHours(8D)).UtcDateTime;

		return specifyDate;
	}

	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
	{
		var outputDate = new DateTimeOffset(value).ToOffset(TimeSpan.FromHours(8D));
		writer.WriteStringValue(outputDate.ToString("yyyyMMddHHmmss"));
	}
}
