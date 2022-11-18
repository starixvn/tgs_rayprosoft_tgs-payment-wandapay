namespace Tgs.Payment.WandaPay.Client;
public record WandaPayWithdrawApplyInfo(
	WandaPayWithdrawSettings Settings,
	WandaPayWithdrawResponse Response);
