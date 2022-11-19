using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Stateless;
using Tgs.Payment.Clients;
using Tgs.Payment.WandaPay;
using Tgs.Payment.WandaPay.ApplicationServices;
using Tgs.Payment.WandaPay.Client;
using Tgs.ServiceUtility;
using Xunit.Abstractions;

namespace WandaPay.IntegrationTests;

public class DepositTests
{
	public ITestOutputHelper TestOutputHelper { get; }

	public DepositTests(ITestOutputHelper testOutputHelper)
	{
		TestOutputHelper = testOutputHelper;
	}

	[Fact]
	public async Task DepositCallback()
	{
		var fakeGetDepositFormService = Substitute.For<IGetService<GetDepositFormQuery, DepositForm?>>();
		var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
		var fakePaymentClient = Substitute.For<IPaymentClient>();

		await using var web = WandaPayTestHost.Factory
			.WithWebHostBuilder(hostBuilder => hostBuilder
				.ConfigureLogging(logging => logging.AddXUnit(TestOutputHelper))
				.ConfigureServices(services => services
					.AddSingleton(fakeGetDepositFormService)
					.AddSingleton(fakePaymentServiceClient)
					.AddSingleton(fakePaymentClient)));

		var httpClient = web.CreateClient();

		var depositFormId = Guid.Parse("a33b98eb-80f9-4cb2-b823-6f4fe817d0b7");
		var depositResult = JsonNode.Parse("""
			{
				"random": "23e8f41179fe4324bc0446b8d001fb15",
				"storeOrderNo": "a33b98eb-80f9-4cb2-b823-6f4fe817d0b7",
				"payResult": "success",
				"orderNo": "Y20221163787B82E4B0B09A4435C64E",
				"totalAmt": "500.00",
				"sign": "7FD8305942FE3A6211F71A7B9B729F47",
				"payStartTime": "20221119144522",
				"payEndTime": "20221119144653",
				"storeCode": "80059"
			}
			""");

		var applyInfo = new WandaPayDepositApplyInfo(
			new WandaPayDepositSettings(
				"80059",
				"lgL0XZoapjTeoFDmr6xvg8XFvkoHOOgPgvMBHYgCekxU0pOj2IoQvWhmHT4PM1Qb",
				PayType.BankCard,
				depositFormId,
				"測試一"),
			new WandaPayDepositResponse(
				"80059",
				"Y20221163787B82E4B0B09A4435C64E",
				depositFormId,
				"pending",
				500M,
				new Uri("http://localhost")));

		_ = fakeGetDepositFormService.GetAsync(Arg.Any<GetDepositFormQuery>())
			.Returns(new DepositForm(
				depositFormId,
				"player1",
				Guid.NewGuid(),
				Guid.NewGuid(),
				500M,
				PaymentFlowState.WaitPayment,
				applyInfo));

		var fsm = new StateMachine<PaymentFlowState, PaymentTrigger>(PaymentFlowState.WaitPayment);

		_ = fsm.Configure(PaymentFlowState.Init)
			.Permit(PaymentTrigger.DepositCreate, PaymentFlowState.WaitPayment)
			.Permit(PaymentTrigger.Reject, PaymentFlowState.Reject);

		_ = fsm.Configure(PaymentFlowState.WaitPayment)
			.Permit(PaymentTrigger.PaymentDepositCompleted, PaymentFlowState.WaitApprove)
			.Permit(PaymentTrigger.Reject, PaymentFlowState.Reject);

		_ = fsm.Configure(PaymentFlowState.WaitApprove)
			.Permit(PaymentTrigger.Approve, PaymentFlowState.Approved)
			.Permit(PaymentTrigger.Reject, PaymentFlowState.Reject);

		_ = fakePaymentServiceClient.CreateDepositStateMachine(Arg.Is(PaymentFlowState.WaitPayment))
			.Returns(fsm);

		var response = await httpClient.PostAsJsonAsync("/deposit/callback", depositResult);

		var actual = await response.Content.ReadAsStringAsync();

		Assert.Equal("success", actual);

		_ = fakePaymentServiceClient.Received(1)
			.SubmitDepositFormStatusAsync(
				Arg.Is(depositFormId),
				Arg.Is((PaymentFlowState?)null),
				Arg.Any<string>(),
				cancellationToken: Arg.Any<CancellationToken>());
		_ = fakePaymentClient.Received(1)
			.ApproveDepositAsync(
				Arg.Is(depositFormId),
				Arg.Any<Guid>(),
				Arg.Is(500M),
				Arg.Any<CancellationToken>());
	}
}
