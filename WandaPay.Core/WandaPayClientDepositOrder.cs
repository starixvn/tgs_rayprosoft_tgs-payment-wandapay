using System.Text.Json.Serialization;

namespace Tgs.Payment.WandaPay;
public record WandaPayClientDepositOrder(
	[property:JsonPropertyName("storeOrderNo")]
	Guid FormId,
	string PayType,
	[property:JsonPropertyName("totalAmt")]
	decimal Amount,
	Uri BackUrl,
	Uri NotifyUrl,
	string? PlayerName);
