using System.Net.Http.Json;
using System.Text.Json;
using Tgs.Payment.WandaPay.Client;

namespace Tgs.Payment.WandaPay;

internal class WandaPayClient : IWandaPayClient
{
	private readonly HttpClient _httpClient;
	private readonly WandaPayHasher _wandaPayHasher;

	public WandaPayClient(HttpClient httpClient, WandaPayHasher wandaPayHasher)
	{
		_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		_wandaPayHasher = wandaPayHasher ?? throw new ArgumentNullException(nameof(wandaPayHasher));
	}

	public ValueTask<WandaPayClientDepositResponse> DepositAsync(
		string storeCode,
		string signKey,
		WandaPayClientDepositOrder order,
		CancellationToken cancellationToken = default)
	{
		var depositOrderNode = JsonSerializer.SerializeToNode(order, WandaPayUtility.SerializerOptions)!;

		depositOrderNode.AsObject().Add("storeCode", storeCode);

		var signValue = _wandaPayHasher.ComputeHash(signKey, depositOrderNode);

		depositOrderNode.AsObject()["sign"] = signValue;

		return _httpClient
			.PostAsJsonAsync(
				"/pay/unifiedorder",
				depositOrderNode,
				WandaPayUtility.SerializerOptions,
				cancellationToken)
			.ToAsyncEnumerable()
			.Do(response => response.EnsureSuccessStatusCode())
			.SelectMany(response => response.Content
				.ReadFromJsonAsync<WandaPayClientDepositResponse>(WandaPayUtility.SerializerOptions, cancellationToken)
				.ToAsyncEnumerable())
			.Where(depositResponse => depositResponse is not null)
			.Select(depositResponse => depositResponse!)
			.Do(depositResponse =>
			{
				if (depositResponse.ErrorCode is not null)
					throw new WandaApiException(
						$"Deposit fail. " +
						$"Return: {JsonSerializer.Serialize(depositResponse, WandaPayUtility.SerializerOptions)}");

				var sign = _wandaPayHasher.ComputeHash(
					signKey,
					JsonSerializer.SerializeToNode(depositResponse, WandaPayUtility.SerializerOptions)!);

				if (sign != depositResponse.Sign)
					throw new WandaApiException("Deposit response sign invald.");
			})
			.SingleAsync(cancellationToken);
	}

	public ValueTask<WandaPayClientWithdrawResponse> WithdrawAsync(string storeCode, string signKey, WandaPayClientWithdrawOrder order, CancellationToken cancellationToken = default)
	{
		var withdrawOrderNode = JsonSerializer.SerializeToNode(order, WandaPayUtility.SerializerOptions)!;

		withdrawOrderNode.AsObject().Add("storeCode", storeCode);

		var signValue = _wandaPayHasher.ComputeHash(signKey, withdrawOrderNode);

		withdrawOrderNode.AsObject()["sign"] = signValue;

		return _httpClient
			.PostAsJsonAsync(
				"/withdraw/unifiedorder",
				withdrawOrderNode,
				WandaPayUtility.SerializerOptions,
				cancellationToken)
			.ToAsyncEnumerable()
			.Do(response => response.EnsureSuccessStatusCode())
			.SelectMany(response => response.Content
				.ReadFromJsonAsync<WandaPayClientWithdrawResponse>(WandaPayUtility.SerializerOptions, cancellationToken)
				.ToAsyncEnumerable())
			.Where(withdrawResponse => withdrawResponse is not null)
			.Select(withdrawResponse => withdrawResponse!)
			.Do(withdrawResponse =>
			{
				if (withdrawResponse.ErrorCode is not null)
					throw new WandaApiException(
						$"Withdraw fail. " +
						$"Return: {JsonSerializer.Serialize(withdrawResponse, WandaPayUtility.SerializerOptions)}");

				var sign = _wandaPayHasher.ComputeHash(
					signKey,
					JsonSerializer.SerializeToNode(withdrawResponse, WandaPayUtility.SerializerOptions)!);

				if (sign != withdrawResponse.Sign)
					throw new WandaApiException("Withdraw response sign invald.");
			})
			.SingleAsync(cancellationToken);
	}
}
