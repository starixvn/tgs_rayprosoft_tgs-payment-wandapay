namespace Tgs.Payment.WandaPay.Client;
public record WandaPayWithdrawResponse(
	string StoreCode,
	string OrderNo,
	Guid FormId,
	string PayResult,
	decimal Amount,
	DateTime PayStartTime,
	DateTime? PayEndTime);
