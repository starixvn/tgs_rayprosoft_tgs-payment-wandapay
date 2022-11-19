using Tgs.Payment.Clients;
using Tgs.Payment.WandaPay.Client;

namespace Tgs.Payment.WandaPay;
public record WithdrawForm(
	Guid Id,
	string? UserId,
	UserType UserType,
	Guid BrandId,
	Guid? BrandPaymentId,
	decimal Amount,
	PaymentFlowState Status,
	Guid BankInfoId,
	WandaPayWithdrawApplyInfo? ApplyInfo,
	string? PaymentReturn = null);
