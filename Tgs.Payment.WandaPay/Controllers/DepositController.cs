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
		// {"random":"23e8f41179fe4324bc0446b8d001fb15","storeOrderNo":"a33b98eb-80f9-4cb2-b823-6f4fe817d0b7","payResult":"success","orderNo":"Y20221163787B82E4B0B09A4435C64E","totalAmt":"500.00","sign":"7FD8305942FE3A6211F71A7B9B729F47","payStartTime":"20221119144522","payEndTime":"20221119144653","storeCode":"80059"}
		_logger.LogInformation("{depositResult}", depositResult);

		return "success";
	}
}
