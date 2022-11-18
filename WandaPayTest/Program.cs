using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tgs.Payment.Clients;
using Tgs.Payment.WandaPay.Client;

var builder = Host.CreateDefaultBuilder(args)
	.ConfigureLogging((_, logging) => logging
		.AddFilter("Tgs", LogLevel.Trace))
	.ConfigureServices(services => services
		.AddPaymentClient()
		.AddMessageQueue()
		.AddNatsMessageQueue(natsConfig => natsConfig
			.ConfigQueueOptions((natsOpt, _) => natsOpt.Url = "nats://localhost:4222"))
		.AddNatsGlobPatternExchange("*"));

var app = builder.Build();

var paymentClient = app.Services.GetRequiredService<IPaymentClient>();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("start");

var signKey = "lgL0XZoapjTeoFDmr6xvg8XFvkoHOOgPgvMBHYgCekxU0pOj2IoQvWhmHT4PM1Qb";
var storeCode = "80059";

var depositSettings = new WandaPayDepositSettings(
	storeCode,
	signKey,
	PayType.BankCard,
	Guid.NewGuid(),
	"測試一");

var depositReturn = await paymentClient.DepositAsync(
	"tgs.payment.wandapay.deposit",
	500M,
	JsonSerializer.Serialize(depositSettings, WandaPayUtility.SerializerOptions)).ConfigureAwait(false);

logger.LogInformation("{depositReturn}", depositReturn);

//var withdrawOrder = new WandaPayWithdrawOrder(
//	Guid.NewGuid(),
//	"測試一",
//	"11111111",
//	500M,
//	new Uri("http://localhost"),
//	"ICBC");

//var withdrawResponse = await client.WithdrawAsync(storeCode, signKey, withdrawOrder);

//Console.WriteLine(withdrawResponse);
