using System.Net.Http.Json;
using System.Text.Json;

namespace Tgs.Payment.WandaPay;

internal class WandaPayClient
{
	private readonly HttpClient _httpClient;
	private readonly WandaPayHasher _wandaPayHasher;

	public WandaPayClient(HttpClient httpClient, WandaPayHasher wandaPayHasher)
	{
		_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		_wandaPayHasher = wandaPayHasher ?? throw new ArgumentNullException(nameof(wandaPayHasher));
	}

	public ValueTask<WandaPayDepositResponse> DepositAsync(string storeCode, string signKey, WandaPayDepositOrder order, CancellationToken cancellationToken = default)
	{
		var depositOrderNode = JsonSerializer.SerializeToNode(order, WandaPayUtility.SerializerOptions)!;

		depositOrderNode.AsObject().Add("storeCode", storeCode);

		var signValue = _wandaPayHasher.ComputeHash(signKey, depositOrderNode);

		depositOrderNode.AsObject()["sign"] = signValue;

		return _httpClient.PostAsJsonAsync("/pay/unifiedorder", depositOrderNode, WandaPayUtility.SerializerOptions, cancellationToken)
			.ToAsyncEnumerable()
			.Do(response => response.EnsureSuccessStatusCode())
			.SelectMany(response => response.Content
				.ReadFromJsonAsync<WandaPayDepositResponse>(WandaPayUtility.SerializerOptions, cancellationToken)
				.ToAsyncEnumerable())
			.Where(depositResponse => depositResponse is not null)
			.Select(depositResponse => depositResponse!)
			.SingleAsync(cancellationToken);
	}
}
