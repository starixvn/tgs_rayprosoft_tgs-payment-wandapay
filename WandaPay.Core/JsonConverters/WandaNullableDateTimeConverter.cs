using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.Payment.WandaPay.JsonConverters;

public class WandaNullableDateTimeConverter : JsonConverter<DateTime?>
{
	public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var stringValue = reader.GetString();

		if (string.IsNullOrEmpty(stringValue))
			return null;

		var unspecifyDate = DateTime.ParseExact(stringValue, "yyyyMMddHHmmss", CultureInfo.CurrentCulture);

		var specifyDate = new DateTimeOffset(
			unspecifyDate,
			TimeSpan.FromHours(8D)).UtcDateTime;

		return specifyDate;
	}

	public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
	{
		if (value is null)
		{
			writer.WriteNullValue();
			return;
		}

		var outputDate = new DateTimeOffset(value.Value).ToOffset(TimeSpan.FromHours(8D));
		writer.WriteStringValue(outputDate.ToString("yyyyMMddHHmmss"));
	}
}
