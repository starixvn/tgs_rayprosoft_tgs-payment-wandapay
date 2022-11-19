using NSubstitute;
using Tgs.Payment.Clients;
using Tgs.Payment.WandaPay.ApplicationServices;
using Xunit;

namespace WandaPay.UnitTests;

public class PaymentTests
{
	[Fact]
	public async Task 取得DepositForm資料()
	{
		// Arrange
		var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();

		var sut = new GetDepositFormQueryService(fakePaymentServiceClient);

		var query = new GetDepositFormQuery(Guid.NewGuid());

		// Act
		_ = await sut.GetAsync(query);

		// Assert
		_ = fakePaymentServiceClient.Received(1)
			.GetDepositFormAsync(Arg.Is(query.FormId));
	}

	[Fact]
	public async Task 取得WithdrawForm資料()
	{
		// Arrange
		var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();

		var sut = new GetWithdrawFormQueryService(fakePaymentServiceClient);

		var query = new GetWithdrawFormQuery(Guid.NewGuid());

		// Act
		_ = await sut.GetAsync(query);

		// Assert
		_ = fakePaymentServiceClient.Received(1)
			.GetWithdrawFormAsync(Arg.Is(query.FormId));
	}
}
