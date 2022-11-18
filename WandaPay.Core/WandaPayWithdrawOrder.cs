using System.Text.Json.Serialization;

namespace Tgs.Payment.WandaPay;
public record WandaPayWithdrawOrder(
	[property: JsonPropertyName("storeOrderNo")]
	Guid FormId,
	string AccountName,
	string AccountNo,
	[property: JsonPropertyName("transAmt")]
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
