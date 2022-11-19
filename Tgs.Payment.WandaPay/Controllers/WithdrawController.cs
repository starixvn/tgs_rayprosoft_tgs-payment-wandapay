using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;

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
	public string WithdrawCallback(
		JsonNode withdrawResult)
	{
		_logger.LogInformation("{withdrawResult}", withdrawResult);

		return "success";
	}
}
