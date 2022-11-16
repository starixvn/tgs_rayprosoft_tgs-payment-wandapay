using System.Numerics;
using System.Security.Cryptography;
using System.Text;

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
}