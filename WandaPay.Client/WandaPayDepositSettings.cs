namespace Tgs.Payment.WandaPay.Client;
public record WandaPayDepositSettings(
	string StoreCode,
	string SignKey,
	string PayType,
	Guid FormId,
	string RealName);
