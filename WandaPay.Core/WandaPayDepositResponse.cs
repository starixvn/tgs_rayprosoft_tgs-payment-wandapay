using System.Text.Json.Serialization;

namespace Tgs.Payment.WandaPay;
public record WandaPayDepositResponse(
	string ErrorCode,
	[property: JsonPropertyName("errorMsg")]
	string? ErrorMessage);
