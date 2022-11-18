using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Tgs.Payment.WandaPay;
using Tgs.Payment.WandaPay.Client;

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
}
