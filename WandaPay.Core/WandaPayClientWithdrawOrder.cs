using System.Text.Json.Serialization;
using Tgs.Payment.WandaPay.JsonConverters;

namespace Tgs.Payment.WandaPay;
public record WandaPayClientWithdrawOrder(
	[property: JsonPropertyName("storeOrderNo")]
	Guid FormId,
	string AccountName,
	string AccountNo,
	[property: JsonPropertyName("transAmt")]
	[property: JsonConverter(typeof(WandaDecimalConverter))]
	decimal TransferAmount,
	Uri NotifyUrl,
	string BankCode,
	string? BankBranchName = null,
	int? BankAccountType = null,
	string? Province = null,
	string? City = null,
	string? UserIdCard = null,
	string? UserMobile = null,
	string? TransMemo = null);
