using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.Payment.WandaPay;

public static class WandaPayUtility
{
	public static readonly JsonSerializerOptions SerializerOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
	};
}
