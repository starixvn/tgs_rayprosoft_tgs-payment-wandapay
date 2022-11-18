namespace Tgs.Payment.WandaPay;

internal interface IWandaPayClient
{
	ValueTask<WandaPayClientDepositResponse> DepositAsync(string storeCode, string signKey, WandaPayClientDepositOrder order, CancellationToken cancellationToken = default);
	ValueTask<WandaPayClientWithdrawResponse> WithdrawAsync(string storeCode, string signKey, WandaPayClientWithdrawOrder order, CancellationToken cancellationToken = default);
}