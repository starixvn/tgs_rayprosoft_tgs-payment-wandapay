using System.Text.Json;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using Tgs.MessageQueue;
using Tgs.Payment.Clients;
using Tgs.Payment.Protos;
using Tgs.Payment.WandaPay.Client;

namespace Tgs.Payment.WandaPay.MessageQueue;

internal class WithdrawProcessor : IMessageProcessor
{
	private readonly WandaPayOptions _options;
	private readonly IWandaPayClient _wandaPayClient;
	private readonly IPaymentClient _paymentClient;

	public WithdrawProcessor(
		IWandaPayClient wandaPayClient,
		IPaymentClient paymentClient,
		IOptions<WandaPayOptions> optionsAccessor)
	{
		_options = optionsAccessor.Value;
		_wandaPayClient = wandaPayClient ?? throw new ArgumentNullException(nameof(wandaPayClient));
		_paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
	}

	public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
	{
		var request = WithdrawRequest.Parser.ParseFrom(data.Span);
		var settings = JsonSerializer.Deserialize<WandaPayWithdrawSettings>(request.ApplyInfo, WandaPayUtility.SerializerOptions)!;

		var order = new WandaPayClientWithdrawOrder(
			settings.FormId,
			settings.AccountName,
			settings.AccountNo,
			decimal.Parse(request.Amount),
			new Uri($"{_options.CallbackBaseUrl}/withdraw/callback"),
			settings.BankCode,
			settings.BankBranchName,
			settings.BankAccountType,
			settings.Province,
			settings.City,
			settings.UserIdCard,
			settings.UserMobile,
			settings.TransMemo);

		var withdrawResponse = await _wandaPayClient.WithdrawAsync(
			settings.StoreCode,
			settings.SignKey,
			order,
			cancellationToken).ConfigureAwait(false);

		var applyInfo = new WandaPayWithdrawApplyInfo(
			settings,
			new WandaPayWithdrawResponse(
				withdrawResponse.StoreCode,
				withdrawResponse.OrderNo,
				withdrawResponse.FormId,
				withdrawResponse.PayResult,
				withdrawResponse.Amount,
				withdrawResponse.PayStartTime,
				withdrawResponse.PayEndTime));

		var thirdPartyOrderNo = applyInfo.Response.OrderNo;
		await _paymentClient
			.SaveWithdrawFormThirdPartyOrderNoAsync(
				settings.FormId,
				thirdPartyOrderNo,
				cancellationToken)
			.ConfigureAwait(false);

		var msg = new WithdrawResponse
		{
			Result = JsonSerializer.Serialize(applyInfo, WandaPayUtility.SerializerOptions)
		};

		return msg.ToByteArray();
	}
}
