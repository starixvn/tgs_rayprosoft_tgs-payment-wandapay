using System.Text.Json;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.Payment.Clients;
using Tgs.Payment.Protos;
using Tgs.Payment.WandaPay;
using Tgs.Payment.WandaPay.Client;
using Tgs.Payment.WandaPay.MessageQueue;

namespace WandaPay.UnitTests;

public class MessageQueueTests
{
	[Fact]
	public async Task DepositProcessor_可以向三方申請代收()
	{
		// Arrange
		var fakeWandaClient = Substitute.For<IWandaPayClient>();
		var optionsAccessor = new OptionsWrapper<WandaPayOptions>(new WandaPayOptions
		{
			CallbackBaseUrl = "https://dev-payment.rpgp.cc/wandapay-test",
			PaymentApiUrl = new Uri("http://localhost"),
			PaymentServiceBaseUrl = new Uri("http://localhost")
		});

		var sut = new DepositProcessor(fakeWandaClient, optionsAccessor);

		var storeCode = "12345";
		var signKey = "abcdefg";

		var depositSettings = new WandaPayDepositSettings(
			storeCode,
			signKey,
			PayType.BankCard,
			Guid.NewGuid(),
			"測試一");

		var request = new DepositRequest
		{
			Amount = "100",
			ApplyInfo = JsonSerializer.Serialize(depositSettings, WandaPayUtility.SerializerOptions)
		};

		var expectedOrder = new WandaPayClientDepositOrder(
			depositSettings.FormId,
			PayType.BankCard,
			100M,
			new Uri("http://localhost"),
			new Uri("https://dev-payment.rpgp.cc/wandapay-test/deposit/callback"),
			depositSettings.RealName);

		_ = fakeWandaClient.DepositAsync(
			Arg.Is(storeCode),
			Arg.Is(signKey),
			Arg.Is(expectedOrder),
			Arg.Any<CancellationToken>())
			.Returns(new WandaPayClientDepositResponse(
				null,
				null,
				storeCode,
				"Y20221163771605E4B0B09A4435C3B0",
				depositSettings.FormId,
				"pending",
				100M,
				"http://localhost",
				string.Empty,
				string.Empty));

		var expected = new WandaPayDepositApplyInfo(
			depositSettings,
			new WandaPayDepositResponse(
				storeCode,
				"Y20221163771605E4B0B09A4435C3B0",
				depositSettings.FormId,
				"pending",
				100M,
				"http://localhost"));

		// Act
		var result = await sut.HandleAsync(request.ToByteArray());
		var paymentReturn = DepositResponse.Parser.ParseFrom(result.Span)!;
		var actual = JsonSerializer.Deserialize<WandaPayDepositApplyInfo>(paymentReturn.Result, WandaPayUtility.SerializerOptions);

		// Assert
		Assert.NotNull(actual);
		Assert.Equal(expected, actual);
	}

	[Fact]
	public async Task WithdrawProcessor_可以向三方申請代付()
	{
		// Arrange
		var fakeWandaClient = Substitute.For<IWandaPayClient>();
		var fakePaymentClient = Substitute.For<IPaymentClient>();
		var optionsAccessor = new OptionsWrapper<WandaPayOptions>(new WandaPayOptions
		{
			CallbackBaseUrl = "https://dev-payment.rpgp.cc/wandapay-test",
			PaymentApiUrl = new Uri("http://localhost"),
			PaymentServiceBaseUrl = new Uri("http://localhost")
		});

		var sut = new WithdrawProcessor(fakeWandaClient, fakePaymentClient, optionsAccessor);

		var storeCode = "12345";
		var signKey = "abcdefg";

		var withdrawSettings = new WandaPayWithdrawSettings(
			storeCode,
			signKey,
			Guid.NewGuid(),
			"測試一",
			"11111111",
			"ICBC");

		var request = new WithdrawRequest
		{
			Amount = "100",
			ApplyInfo = JsonSerializer.Serialize(withdrawSettings, WandaPayUtility.SerializerOptions)
		};

		var expectedOrder = new WandaPayClientWithdrawOrder(
			withdrawSettings.FormId,
			withdrawSettings.AccountName,
			withdrawSettings.AccountNo,
			100M,
			new Uri("https://dev-payment.rpgp.cc/wandapay-test/withdraw/callback"),
			withdrawSettings.BankCode,
			withdrawSettings.BankBranchName,
			withdrawSettings.BankAccountType,
			withdrawSettings.Province,
			withdrawSettings.City,
			withdrawSettings.UserIdCard,
			withdrawSettings.UserMobile,
			withdrawSettings.TransMemo);

		var payStartTime = DateTime.Now;

		_ = fakeWandaClient.WithdrawAsync(
			Arg.Is(storeCode),
			Arg.Is(signKey),
			Arg.Is(expectedOrder),
			Arg.Any<CancellationToken>())
			.Returns(new WandaPayClientWithdrawResponse(
				null,
				null,
				storeCode,
				"Y20221163771605E4B0B09A4435C3B0",
				withdrawSettings.FormId,
				"pending",
				100M,
				payStartTime,
				null,
				string.Empty,
				string.Empty));

		var expected = new WandaPayWithdrawApplyInfo(
			withdrawSettings,
			new WandaPayWithdrawResponse(
			storeCode,
			"Y20221163771605E4B0B09A4435C3B0",
			withdrawSettings.FormId,
			"pending",
			100M,
			payStartTime,
			null));

		// Act
		var result = await sut.HandleAsync(request.ToByteArray());
		var paymentReturn = DepositResponse.Parser.ParseFrom(result.Span)!;
		var actual = JsonSerializer.Deserialize<WandaPayWithdrawApplyInfo>(paymentReturn.Result, WandaPayUtility.SerializerOptions);

		// Assert
		Assert.NotNull(actual);
		Assert.Equal(expected, actual);

		_ = fakePaymentClient.Received(1)
			.SaveWithdrawFormThirdPartyOrderNoAsync(
				Arg.Is(withdrawSettings.FormId),
				Arg.Is("Y20221163771605E4B0B09A4435C3B0"));
	}
}
