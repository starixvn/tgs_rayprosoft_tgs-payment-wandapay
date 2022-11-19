using System.Text.Json.Serialization;
using Tgs.Payment.WandaPay.JsonConverters;

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

	[JsonConverter(typeof(WandaDateTimeConverter))]
	public DateTime PayStartTime { get; set; }

	[JsonConverter(typeof(WandaNullableDateTimeConverter))]
	public DateTime? PayEndTime { get; set; }
}
