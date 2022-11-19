using Tgs.Payment.Clients;
using Tgs.Payment.WandaPay.Client;

namespace Tgs.Payment.WandaPay;
public record DepositForm(
	Guid Id,
	string PlayerCode,
	Guid BrandId,
	Guid BrandPaymentId,
	decimal Amount,
	PaymentFlowState Status,
	WandaPayDepositApplyInfo? ApplyInfo,
	string? PaymentReturn = null);
