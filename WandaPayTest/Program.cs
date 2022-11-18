using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tgs.Payment.WandaPay;

var builder = Host.CreateDefaultBuilder(args)
	.ConfigureLogging((_, logging) => logging
		.AddFilter("Tgs", LogLevel.Trace))
	.ConfigureServices(services => services
		.AddSingleton<WandaPayHasher>()
		.AddHttpClient<WandaPayClient>()
		.ConfigureHttpClient(http => http.BaseAddress = new Uri("https://xapi.z1wx8.com"))
		.AddLoggingHandler<WandaPayClient>());

var app = builder.Build();

var client = app.Services.GetRequiredService<WandaPayClient>();

var signKey = "lgL0XZoapjTeoFDmr6xvg8XFvkoHOOgPgvMBHYgCekxU0pOj2IoQvWhmHT4PM1Qb";
var storeCode = "80059";

var depositOrder = new WandaPayDepositOrder(
	Guid.NewGuid(),
	PayType.BankCard,
	601M,
	new Uri("http://localhost"),
	new Uri("http://localhost"),
	"測試一");

var response = await client.DepositAsync(storeCode, signKey, depositOrder);

Console.WriteLine(response);

//var withdrawOrder = new WandaPayWithdrawOrder(
//	Guid.NewGuid(),
//	"測試一",
//	"11111111",
//	500M,
//	new Uri("http://localhost"),
//	"ICBC");

//var withdrawResponse = await client.WithdrawAsync(storeCode, signKey, withdrawOrder);

//Console.WriteLine(withdrawResponse);
