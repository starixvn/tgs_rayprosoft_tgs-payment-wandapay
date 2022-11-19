using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.Clients;
using Tgs.Payment.WandaPay.ApplicationServices;
using Tgs.Payment.WandaPay.Client;
using Tgs.Payment.WandaPay.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.Payment.WandaPay.Controllers;

[Route("[controller]")]
[ApiController]
public class WithdrawController : ControllerBase
{
	private readonly ILogger<WithdrawController> _logger;

	public WithdrawController(ILogger<WithdrawController> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	[HttpPost("callback", Name = "NotifyWithdraw")]
	public async Task<string> WithdrawCallbackAsync(
		[FromServices] IServiceCoordinator serviceCoordinator,
		[FromServices] WandaPayHasher wandaPayHasher,
		[FromServices] IPaymentServiceClient paymentServiceClient,
		[FromServices] IPaymentClient paymentClient,
		JsonNode withdrawResult)
	{
		var viewModel = JsonSerializer.Deserialize<WithdrawCallbackViewModel>(
			withdrawResult,
			WandaPayUtility.SerializerOptions);

		if (viewModel is null)
		{
			_logger.LogWarning("Parse request is null. {node}", withdrawResult);
			return "fail";
		}

		var withdrawForm = await serviceCoordinator
			.GetAsync<GetWithdrawFormQuery, WithdrawForm?>(new GetWithdrawFormQuery(viewModel.FormId))
			.ConfigureAwait(false);

		if (withdrawForm is null)
		{
			_logger.LogWarning("Can't found withdraw form({id}).", viewModel.FormId);
			return "fail";
		}

		var sign = wandaPayHasher.ComputeHash(
			withdrawForm.ApplyInfo!.Settings.SignKey,
			withdrawResult);

		if (sign != withdrawResult["sign"]?.GetValue<string>())
		{
			_logger.LogWarning("Sign value invalid. input: {input}, compute: {sign}", withdrawResult["sign"], sign);
			return "fail";
		}

		var fsm = paymentServiceClient.CreateWithdrawStateMachine(withdrawForm.Status);

		switch (viewModel.PayResult)
		{
			case "success":
				if (fsm.CanFire(PaymentTrigger.PaymentWithdrawCompleted))
				{
					await fsm.FireAsync(PaymentTrigger.PaymentWithdrawCompleted).ConfigureAwait(false);

					await paymentServiceClient.SubmitWithdrawFormStatusAsync(
						withdrawForm.Id,
						paymentReturn: JsonSerializer.Serialize(
							viewModel,
							WandaPayUtility.SerializerOptions),
						cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

					await paymentClient.PaidWithdrawAsync(
						withdrawForm.Id,
						withdrawForm.BrandId,
						HttpContext.RequestAborted).ConfigureAwait(false);
				}

				break;
		}

		return "success";
	}
}
