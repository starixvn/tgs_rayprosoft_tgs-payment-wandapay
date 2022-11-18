using System.Text.Json.Serialization;

namespace Tgs.Payment.WandaPay.ViewModels;

public class DepositCallbackViewModel
{
	public required string StoreCode { get; set; }

	public required string OrderNo { get; set; }

	[JsonPropertyName("storeOrderNo")]
	public required Guid FormId { get; set; }

	public required string PayResult { get; set; }

	[JsonPropertyName("totalAmt")]
	public decimal Amount { get; set; }

	public DateTime PayStartTime { get; set; }

	public DateTime? PayEndTime { get; set; }
}
