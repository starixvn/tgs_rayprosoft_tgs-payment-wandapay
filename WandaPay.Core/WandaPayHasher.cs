using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

namespace Tgs.Payment.WandaPay;

public class WandaPayHasher
{
	private readonly ILogger<WandaPayHasher> _logger;

	public WandaPayHasher(ILogger<WandaPayHasher> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public string ComputeHash(string signKey, JsonNode data)
	{
		var stringToSign = string.Join(
			"&",
			data.AsObject()
				.AsEnumerable()
				.Where(prop => prop.Key is not "sign"
					&& !string.IsNullOrEmpty(prop.Value?.ToString()))
				.OrderBy(prop => prop.Key)
				.Select(prop => $"{prop.Key}={prop.Value}")
				.Concat(new[] { $"key={signKey}" }));

		_logger.LogTrace("StringToSign={stringToSign}", stringToSign);

		var hash = MD5.HashData(Encoding.UTF8.GetBytes(stringToSign));

		return new BigInteger(hash, true, true).ToString("X32");
	}
}
