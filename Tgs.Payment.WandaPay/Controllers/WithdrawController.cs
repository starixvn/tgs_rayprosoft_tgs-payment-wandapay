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
		// {"random":"508396fe0d8c474aa9acaf5f2254010d","storeOrderNo":"dcb95746-ce8d-4da2-a8f4-10dfd3f748a9","payResult":"success","orderNo":"Y20221163789C2EE4B0B09A4435C9CD","totalAmt":"500.00","sign":"92ACD661E09E8185D7CB71DDEFC6D623","payStartTime":"20221119170446","payEndTime":"20221119170549","storeCode":"80059"}
		_logger.LogInformation("{withdrawResult}", withdrawResult);

		return "success";
	}
}
