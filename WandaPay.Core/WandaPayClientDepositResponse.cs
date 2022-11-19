using System.Text.Json.Serialization;

namespace Tgs.Payment.WandaPay;
public record WandaPayClientDepositResponse(
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
	string PayUrl,
	string Sign,
	string Random);
