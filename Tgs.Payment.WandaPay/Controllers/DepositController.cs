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
public class DepositController : ControllerBase
{
	private readonly ILogger<DepositController> _logger;

	public DepositController(ILogger<DepositController> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	[HttpPost("callback", Name = "NotifyDeposit")]
	public async Task<string> DepositCallbackAsync(
		[FromServices] WandaPayHasher wandaPayHasher,
		[FromServices] IServiceCoordinator serviceCoordinator,
		[FromServices] IPaymentServiceClient paymentServiceClient,
		[FromServices] IPaymentClient paymentClient,
		JsonNode depositResult)
	{
		var viewModel = JsonSerializer.Deserialize<DepositCallbackViewModel>(
			depositResult,
			WandaPayUtility.SerializerOptions);

		if (viewModel is null)
		{
			_logger.LogWarning("Parse request is null. {node}", depositResult);
			return "fail";
		}

		var depositForm = await serviceCoordinator
			.GetAsync<GetDepositFormQuery, DepositForm>(new GetDepositFormQuery(viewModel.FormId))
			.ConfigureAwait(false);

		if (depositForm == null)
		{
			_logger.LogWarning("Can't found deposit form({id}).", viewModel.FormId);
			return "fail";
		}

		var sign = wandaPayHasher.ComputeHash(
			depositForm.ApplyInfo!.Settings.SignKey,
			depositResult);

		if (sign != depositResult["sign"]?.GetValue<string>())
		{
			_logger.LogWarning("Sign value invalid. input: {input}, compute: {sign}", depositResult["sign"], sign);
			return "fail";
		}

		var fsm = paymentServiceClient.CreateDepositStateMachine(depositForm.Status);

		switch (viewModel.PayResult)
		{
			case "success":
				if (fsm.CanFire(PaymentTrigger.PaymentDepositCompleted))
				{
					await fsm.FireAsync(PaymentTrigger.PaymentDepositCompleted).ConfigureAwait(false);

					await paymentServiceClient.SubmitDepositFormStatusAsync(
						depositForm.Id,
						paymentReturn: JsonSerializer.Serialize(
							viewModel,
							WandaPayUtility.SerializerOptions),
						cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

					await paymentClient.ApproveDepositAsync(
						depositForm.Id,
						depositForm.BrandId,
						viewModel.Amount).ConfigureAwait(false);
				}

				break;
		}

		return "success";
	}
}
