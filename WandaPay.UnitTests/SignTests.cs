using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Tgs.Payment.WandaPay;
using Tgs.Payment.WandaPay.Client;
using Xunit;

namespace WandaPay.UnitTests;

public class SignTests
{
	[Fact]
	public void WandaPay_Sign值運算()
	{
		var signKey = "im_signkey";
		var stringToSign = $"a=1&b=2&c=3&key={signKey}";

		var expected = "7E7D44AD6F954E7E1DFFD29CE407A997";

		var hash = MD5.HashData(Encoding.UTF8.GetBytes(stringToSign));
		var actual = new BigInteger(hash, true, true);

		Assert.Equal(expected, actual.ToString("X32"));
	}

	[Fact]
	public void WandaPay_輸入物件會排序屬性名稱用MD5運算Hash()
	{
		// Arrange
		var sut = new WandaPayHasher(NullLogger<WandaPayHasher>.Instance);
		var data = new
		{
			b = 2,
			a = 1,
			c = 3
		};
		var signKey = "im_signkey";

		var expected = "7E7D44AD6F954E7E1DFFD29CE407A997";

		// Act
		var actual = sut.ComputeHash(signKey, JsonSerializer.SerializeToNode(data)!);

		// Assert
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void WandaPay_屬性是空值時不參與計算Hash()
	{
		// Arrange
		var sut = new WandaPayHasher(NullLogger<WandaPayHasher>.Instance);
		var data = new
		{
			b = 2,
			a = 1,
			c = 3,
			d = string.Empty,
			e = default(string)
		};
		var signKey = "im_signkey";

		var expected = "7E7D44AD6F954E7E1DFFD29CE407A997";

		// Act
		var actual = sut.ComputeHash(signKey, JsonSerializer.SerializeToNode(data)!);

		// Assert
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void WandaPay_DepositResponse計算Hash()
	{
		// Arrange
		var sut = new WandaPayHasher(NullLogger<WandaPayHasher>.Instance);
		var response = new WandaPayClientDepositResponse(
			null,
			null,
			"80059",
			"Y2022116375B30BE4B0B09A4435AB00",
			Guid.Parse("6820ce08-bd2d-4498-bd9c-51b8813d9bb4"),
			"pending",
			601.00M,
			new Uri("https://o.xyxy1688.com/#/notNameBank?ptorder=20221117100000107254153394ba"),
			"748681797BA05CEC88E35DE3D74DCCF7",
			"ce055751cc9140a8ab88843596081f93");
		var signKey = "lgL0XZoapjTeoFDmr6xvg8XFvkoHOOgPgvMBHYgCekxU0pOj2IoQvWhmHT4PM1Qb";

		// Act
		var actual = sut.ComputeHash(signKey, JsonSerializer.SerializeToNode(response, WandaPayUtility.SerializerOptions)!);

		// Assert
		Assert.Equal(response.Sign, actual);
	}

	[Fact]
	public void WandaPay_WithdrawResponse計算Hash()
	{
		// Arrange
		var sut = new WandaPayHasher(NullLogger<WandaPayHasher>.Instance);
		var response = new WandaPayClientWithdrawResponse(
			null,
			null,
			"80059",
			"Y2022116378983EE4B0B09A4435C95B",
			Guid.Parse("2d7ffd7b-9cb5-4cbf-b8fc-8ad3f84ef620"),
			"waiting_review",
			500.00M,
			new DateTimeOffset(
				DateTime.ParseExact("20221119164759", "yyyyMMddHHmmss", CultureInfo.CurrentCulture),
				TimeSpan.FromHours(8D)).UtcDateTime,
			null,
			"C0492736E05B93555B7C8BC6C636C383",
			"165d81b244374e36b9951ad465a95d75");
		var signKey = "lgL0XZoapjTeoFDmr6xvg8XFvkoHOOgPgvMBHYgCekxU0pOj2IoQvWhmHT4PM1Qb";

		// Act
		var actual = sut.ComputeHash(signKey, JsonSerializer.SerializeToNode(response, WandaPayUtility.SerializerOptions)!);

		// Assert
		Assert.Equal(response.Sign, actual);

	}

	[Fact]
	public void WandaPay_Sign值應該字串長度是32碼()
	{
		// Arrange
		var sut = new WandaPayHasher(NullLogger<WandaPayHasher>.Instance);
		var data = new WandaPayClientWithdrawOrder(
			Guid.Parse("c4f614bc-76a0-4654-9b13-ee7f9b6dbe1e"),
			"測試一",
			"1111111",
			500,
			new Uri("https://dev-payment.rpgp.cc/wandapay-test/withdraw/callback"),
			"ICBC");
		var signKey = "lgL0XZoapjTeoFDmr6xvg8XFvkoHOOgPgvMBHYgCekxU0pOj2IoQvWhmHT4PM1Qb";

		var node = JsonSerializer.SerializeToNode(data, WandaPayUtility.SerializerOptions)!;

		node["storeCode"] = "80059";

		var expected = "9FB51F13890D80D6E4030AF0E6607096";

		// Act
		var actual = sut.ComputeHash(signKey, node);

		// Assert
		Assert.Equal(expected, actual);
	}
}
