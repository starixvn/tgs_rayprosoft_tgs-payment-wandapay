using System.Globalization;
using System.Text.Json;
using Tgs.Payment.WandaPay;
using Tgs.Payment.WandaPay.Client;

namespace WandaPay.UnitTests;

public class SerializeTests
{
	[Fact]
	public void WandaPayClientWithdrawResponse反序列化()
	{
		// Arrange
		var json = """
			{
			    "random": "e4b550be496d42418bf0c80e9a1d4e21",
			    "storeOrderNo": "cf20a1b8-9e1b-4401-85ce-4ab29b392c85",
			    "payResult": "waiting_review",
			    "orderNo": "Y202211637893A1E4B0B09A4435C8D1",
			    "totalAmt": "500.00",
			    "sign": "7D72A963D625BF356AB6C6E275712321",
			    "payStartTime": "20221119162818",
			    "payEndTime": "",
			    "storeCode": "80059"
			}
			""";

		var expected = new WandaPayClientWithdrawResponse(
			null,
			null,
			"80059",
			"Y202211637893A1E4B0B09A4435C8D1",
			Guid.Parse("cf20a1b8-9e1b-4401-85ce-4ab29b392c85"),
			"waiting_review",
			500M,
			new DateTimeOffset(
				DateTime.ParseExact("20221119162818", "yyyyMMddHHmmss", CultureInfo.CurrentCulture),
				TimeSpan.FromHours(8D)).UtcDateTime,
			null,
			"7D72A963D625BF356AB6C6E275712321",
			"e4b550be496d42418bf0c80e9a1d4e21");

		// Act
		var actual = JsonSerializer.Deserialize<WandaPayClientWithdrawResponse>(json, WandaPayUtility.SerializerOptions);

		// Assert
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void WandPayClientWithdrawOrder_序列化成JsonNode時Amount去除小數點後的零()
	{
		// Arrange
		var sut = new WandaPayClientWithdrawOrder(
			Guid.NewGuid(),
			"測試者",
			"11111111",
			decimal.Parse("500.000000"),
			new Uri("http://localhost"),
			"ICBC");

		// Act
		var actual = JsonSerializer.SerializeToNode(sut, WandaPayUtility.SerializerOptions);

		// Assert
		Assert.Equal("500", actual?["transAmt"]?.ToString());
	}
}
