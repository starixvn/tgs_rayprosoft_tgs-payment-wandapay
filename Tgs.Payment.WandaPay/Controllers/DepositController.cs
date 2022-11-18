using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;

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

	[HttpPost("callback")]
	public string DepositCallback(
		JsonNode depositResult)
	{
		_logger.LogInformation("{depositResult}", depositResult);

		return "success";
	}
}
