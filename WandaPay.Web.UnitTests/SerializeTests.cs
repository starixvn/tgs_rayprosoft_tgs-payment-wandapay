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

	[Fact]
	public void WithdrawCallback_將要求轉成WithdrawCallbackViewModel()
	{
		// Arrange
		var json = """
			{
			    "random": "508396fe0d8c474aa9acaf5f2254010d",
			    "storeOrderNo": "dcb95746-ce8d-4da2-a8f4-10dfd3f748a9",
			    "payResult": "success",
			    "orderNo": "Y20221163789C2EE4B0B09A4435C9CD",
			    "totalAmt": "500.00",
			    "sign": "92ACD661E09E8185D7CB71DDEFC6D623",
			    "payStartTime": "20221119170446",
			    "payEndTime": "20221119170549",
			    "storeCode": "80059"
			}
			""";
		var node = JsonNode.Parse(json);

		var expected = new WithdrawCallbackViewModel
		{
			StoreCode = "80059",
			OrderNo = "Y20221163789C2EE4B0B09A4435C9CD",
			FormId = Guid.Parse("dcb95746-ce8d-4da2-a8f4-10dfd3f748a9"),
			PayResult = "success",
			Amount = 500M,
			PayStartTime = new DateTimeOffset(
				DateTime.ParseExact("20221119170446", "yyyyMMddHHmmss", CultureInfo.CurrentCulture),
				TimeSpan.FromHours(8D)).UtcDateTime,
			PayEndTime = new DateTimeOffset(
				DateTime.ParseExact("20221119170549", "yyyyMMddHHmmss", CultureInfo.CurrentCulture),
				TimeSpan.FromHours(8D)).UtcDateTime
		};

		// Act
		var actual = JsonSerializer.Deserialize<DepositCallbackViewModel>(node, WandaPayUtility.SerializerOptions);

		// Assert
		expected.ToExpectedObject().ShouldMatch(actual);
	}
}
