using System.Text.Json.Serialization;
using Tgs.Payment.WandaPay.JsonConverters;

namespace Tgs.Payment.WandaPay;
public record WandaPayClientWithdrawResponse(
	string? ErrorCode,
	[property: JsonPropertyName("errorMsg")]
	string? ErrorMessage,
	string StoreCode,
	string OrderNo,
	[property: JsonPropertyName("storeOrderNo")]
	Guid FormId,
	string PayResult,
	[property: JsonPropertyName("totalAmt")]
	decimal Amount,
	[property: JsonConverter(typeof(WandaDateTimeConverter))]
	DateTime PayStartTime,
	[property: JsonConverter(typeof(WandaNullableDateTimeConverter))]
	DateTime? PayEndTime,
	string Sign,
	string Random);
