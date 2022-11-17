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

//{"random":"11c7235192ec404d9f9a1bc13d48f782","storeOrderNo":"b5945c36-0163-49db-a29a-f5a868b5aba3","payResult":"pending","orderNo":"Y2022116375AF0CE4B0B09A4435AA9E","totalAmt":"601.00","sign":"6620D24611A7AB0646715215BE87155B","payUrl":"https://o.haha868.com/#/notNameBank?ptorder=202211171000001731982618b091","storeCode":"80059"}