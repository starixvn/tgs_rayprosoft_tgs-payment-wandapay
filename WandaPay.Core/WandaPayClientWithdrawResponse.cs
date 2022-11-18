using System.Text.Json.Serialization;

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
	DateTime PayStartTime,
	DateTime? PayEndTime,
	string Sign,
	string Random);
