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
//{"random":"e4b550be496d42418bf0c80e9a1d4e21","storeOrderNo":"cf20a1b8-9e1b-4401-85ce-4ab29b392c85","payResult":"waiting_review","orderNo":"Y202211637893A1E4B0B09A4435C8D1","totalAmt":"500.00","sign":"7D72A963D625BF356AB6C6E275712321","payStartTime":"20221119162818","payEndTime":"","storeCode":"80059"}
