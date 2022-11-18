namespace Tgs.Payment.WandaPay.Client;
public record WandaPayWithdrawSettings(
	string StoreCode,
	string SignKey,
	Guid FormId,
	string AccountName,
	string AccountNo,
	string BankCode,
	string? BankBranchName = null,
	int? BankAccountType = null,
	string? Province = null,
	string? City = null,
	string? UserIdCard = null,
	string? UserMobile = null,
	string? TransMemo = null);
