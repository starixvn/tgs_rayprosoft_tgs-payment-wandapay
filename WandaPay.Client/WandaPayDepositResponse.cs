namespace Tgs.Payment.WandaPay.Client;
public record WandaPayDepositResponse(
	string StoreCode,
	string OrderNo,
	Guid FormId,
	string PayResult,
	decimal Amount,
	string PayUrl);
