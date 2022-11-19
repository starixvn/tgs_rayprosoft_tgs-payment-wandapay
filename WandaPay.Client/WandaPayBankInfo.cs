using System.Text.Json.Serialization;

namespace Tgs.Payment.WandaPay.Client;
public record WandaPayBankInfo(
	[property: JsonPropertyName("name")]
	string AccountName,
	[property: JsonPropertyName("bankAccount")]
	string AccountNumber);
