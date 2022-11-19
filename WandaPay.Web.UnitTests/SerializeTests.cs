using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using ExpectedObjects;
using Tgs.Payment.WandaPay.Client;
using Tgs.Payment.WandaPay.ViewModels;

namespace WandaPay.Web.UnitTests;

public class SerializeTests
{
	[Fact]
	public void DepositCallback_將要求轉成DepositCallbackViewModel()
	{
		// Arrange
		var json = """
			{
				"random": "23e8f41179fe4324bc0446b8d001fb15",
				"storeOrderNo": "a33b98eb-80f9-4cb2-b823-6f4fe817d0b7",
				"payResult": "success",
				"orderNo": "Y20221163787B82E4B0B09A4435C64E",
				"totalAmt": "500.00",
				"sign": "7FD8305942FE3A6211F71A7B9B729F47",
				"payStartTime": "20221119144522",
				"payEndTime": "20221119144653",
				"storeCode": "80059"
			}
			""";
		var node = JsonNode.Parse(json);

		var expected = new DepositCallbackViewModel
		{
			StoreCode = "80059",
			FormId = Guid.Parse("a33b98eb-80f9-4cb2-b823-6f4fe817d0b7"),
			OrderNo = "Y20221163787B82E4B0B09A4435C64E",
			Amount = 500M,
			PayResult = "success",
			PayStartTime = new DateTimeOffset(
				DateTime.ParseExact("20221119144522", "yyyyMMddHHmmss", CultureInfo.CurrentCulture),
				TimeSpan.FromHours(8D)).UtcDateTime,
			PayEndTime = new DateTimeOffset(
				DateTime.ParseExact("20221119144653", "yyyyMMddHHmmss", CultureInfo.CurrentCulture),
				TimeSpan.FromHours(8D)).UtcDateTime
		};

		// Act
		var actual = JsonSerializer.Deserialize<DepositCallbackViewModel>(node, WandaPayUtility.SerializerOptions);

		// Assert
		expected.ToExpectedObject().ShouldMatch(actual);
	}
}
