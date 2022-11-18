using System.Text.Json;
using System.Text.Json.Serialization;
using Tgs.Payment.WandaPay;

namespace Tgs.Payment.WandaPay.Client;

public static class WandaPayUtility
{
	public static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
	};
}
