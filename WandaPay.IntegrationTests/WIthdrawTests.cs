using System.Globalization;
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

public class WIthdrawTests
{
	public ITestOutputHelper TestOutputHelper { get; }

	public WIthdrawTests(ITestOutputHelper testOutputHelper)
	{
		TestOutputHelper = testOutputHelper;
	}

	[Fact]
	public async Task WithdrawCallback()
	{
		var fakeGetWithdrawFormService = Substitute.For<IGetService<GetWithdrawFormQuery, WithdrawForm?>>();
		var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
		var fakePaymentClient = Substitute.For<IPaymentClient>();

		await using var web = WandaPayTestHost.Factory
			.WithWebHostBuilder(hostBuilder => hostBuilder
				.ConfigureLogging(logging => logging.AddXUnit(TestOutputHelper))
				.ConfigureServices(services => services
					.AddSingleton(fakeGetWithdrawFormService)
					.AddSingleton(fakePaymentServiceClient)
					.AddSingleton(fakePaymentClient)));

		var httpClient = web.CreateClient();

		var withdrawFormId = Guid.Parse("dcb95746-ce8d-4da2-a8f4-10dfd3f748a9");
		var withdrawResult = JsonNode.Parse("""
			{
			    "random": "508396fe0d8c474aa9acaf5f2254010d",
			    "storeOrderNo": "dcb95746-ce8d-4da2-a8f4-10dfd3f748a9",
			    "payResult": "success",
			    "orderNo": "Y20221163789C2EE4B0B09A4435C9CD",
			    "totalAmt": "500.00",
			    "sign": "92ACD661E09E8185D7CB71DDEFC6D623",
			    "payStartTime": "20221119170446",
			    "payEndTime": "20221119170549",
			    "storeCode": "80059"
			}
			""");

		var applyInfo = new WandaPayWithdrawApplyInfo(
			new WandaPayWithdrawSettings(
				"80059",
				"lgL0XZoapjTeoFDmr6xvg8XFvkoHOOgPgvMBHYgCekxU0pOj2IoQvWhmHT4PM1Qb",
				withdrawFormId,
				"測試一",
				"11111111",
				"ICBC"),
			new WandaPayWithdrawResponse(
				"80059",
				"Y20221163789C2EE4B0B09A4435C9CD",
				withdrawFormId,
				"success",
				500M,
				new DateTimeOffset(
					DateTime.ParseExact("20221119170446", "yyyyMMddHHmmss", CultureInfo.CurrentCulture),
					TimeSpan.FromHours(8D)).UtcDateTime,
				null));

		_ = fakeGetWithdrawFormService.GetAsync(Arg.Any<GetWithdrawFormQuery>())
			.Returns(new WithdrawForm(
				withdrawFormId,
				"player1",
				UserType.Player,
				Guid.NewGuid(),
				Guid.NewGuid(),
				500M,
				PaymentFlowState.Approved,
				Guid.NewGuid(),
				applyInfo,
				null));

		var fsm = new StateMachine<PaymentFlowState, PaymentTrigger>(PaymentFlowState.Approved);

		_ = fsm.Configure(PaymentFlowState.Init)
			.Permit(PaymentTrigger.WithdrawCreate, PaymentFlowState.WaitApprove)
			.Permit(PaymentTrigger.Reject, PaymentFlowState.Reject);

		_ = fsm.Configure(PaymentFlowState.WaitApprove)
			.Permit(PaymentTrigger.Approve, PaymentFlowState.Approved)
			.Permit(PaymentTrigger.Reject, PaymentFlowState.Reject);

		_ = fsm.Configure(PaymentFlowState.Approved)
			.Permit(PaymentTrigger.PaymentWithdrawCompleted, PaymentFlowState.Paid)
			.Permit(PaymentTrigger.Reject, PaymentFlowState.Reject);

		_ = fakePaymentServiceClient.CreateWithdrawStateMachine(Arg.Is(PaymentFlowState.Approved))
			.Returns(fsm);

		var response = await httpClient.PostAsJsonAsync("/withdraw/callback", withdrawResult);

		var actual = await response.Content.ReadAsStringAsync();

		Assert.Equal("success", actual);

		_ = fakePaymentServiceClient.Received(1)
			.SubmitWithdrawFormStatusAsync(
				Arg.Is(withdrawFormId),
				Arg.Is(default(PaymentFlowState?)),
				Arg.Any<string>(),
				cancellationToken: Arg.Any<CancellationToken>());
		_ = fakePaymentClient.Received(1)
			.PaidWithdrawAsync(
				Arg.Is(withdrawFormId),
				Arg.Any<Guid>(),
				Arg.Any<CancellationToken>());
	}
}
