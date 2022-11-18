using System.Text.Json;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using Tgs.MessageQueue;
using Tgs.Payment.Protos;
using Tgs.Payment.WandaPay.Client;

namespace Tgs.Payment.WandaPay.MessageQueue;

internal class DepositProcessor : IMessageProcessor
{
	private readonly IWandaPayClient _wandaPayClient;
	private readonly WandaPayOptions _options;

	public DepositProcessor(
		IWandaPayClient wandaPayClient,
		IOptions<WandaPayOptions> optionsAccessor)
	{
		_wandaPayClient = wandaPayClient ?? throw new ArgumentNullException(nameof(wandaPayClient));
		_options = optionsAccessor.Value;
	}

	public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
	{
		var request = DepositRequest.Parser.ParseFrom(data.Span);

		var settings = JsonSerializer.Deserialize<WandaPayDepositSettings>(
			request.ApplyInfo,
			WandaPayUtility.SerializerOptions)!;

		var order = new WandaPayClientDepositOrder(
			settings.FormId,
			settings.PayType,
			decimal.Parse(request.Amount),
			new Uri("http://localhost"),
			new Uri($"{_options.CallbackBaseUrl}/deposit/callback"),
			settings.RealName);

		var depositResponse = await _wandaPayClient.DepositAsync(
			settings.StoreCode,
			settings.SignKey,
			order,
			cancellationToken).ConfigureAwait(false);

		var applyInfo = new WandaPayDepositApplyInfo(
			settings,
			new WandaPayDepositResponse(
				depositResponse.StoreCode,
				depositResponse.OrderNo,
				depositResponse.FormId,
				depositResponse.PayResult,
				depositResponse.Amount,
				depositResponse.PayUrl));

		var msg = new DepositResponse
		{
			Result = JsonSerializer.Serialize(applyInfo)
		};

		return msg.ToByteArray();
	}
}
