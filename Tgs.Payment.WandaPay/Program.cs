using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.Payment.WandaPay;
using Tgs.Payment.WandaPay.MessageQueue;

var builder = WebApplication.CreateBuilder(args);

builder.Logging
	.AddOpenTelemetry(options =>
	{
		options.IncludeFormattedMessage = true;
		options.ParseStateValues = true;
		_ = options.AddFluentdForwardExporter(fluentOptions =>
		{
			var nodeIp = builder.Configuration.GetValue<string>("NODE_IP")!;
			fluentOptions.Host = nodeIp;
			fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME")!;
			fluentOptions.UseMessagePack();
		});
	});

builder.Services
	.AddControllers();

builder.Services
	.AddWandaPay(
		options => builder.Configuration.GetSection("WandaPay").Bind(options),
		httpClientBuilder => httpClientBuilder
			.ConfigureHttpClient((sp, httpClient) =>
			{
				var options = sp.GetRequiredService<IOptions<WandaPayOptions>>().Value;

				httpClient.BaseAddress = options.PaymentApiUrl;
			})
			.AddLoggingHandler<WandaPayOptions>())
	.AddPaymentClient()
	.AddPaymentServiceClient(httpClientBuilder => httpClientBuilder
		.ConfigureHttpClient((sp, httpClient) =>
		{
			var options = sp.GetRequiredService<IOptions<WandaPayOptions>>().Value;

			httpClient.BaseAddress = options.PaymentServiceBaseUrl;
		})
		.AddLoggingHandler<WandaPayOptions>())
	.AddMessageQueue()
	.AddNatsMessageQueue(natsConfig => natsConfig
		.ConfigQueueOptions((options, _) => options.Url = 
			builder.Configuration.GetConnectionString("Nats")!)
		.AddProcessor(
			ProcessorTypes.DepositProcessorType,
			"tgs.payment.wandapay.deposit",
			"tgs.payment.wandapay.deposit")
		.AddProcessor(
			ProcessorTypes.WithdrawProcessorType,
			"tgs.payment.wandapay.withdraw",
			"tgs.payment.wandapay.withdraw"))
	.AddNatsGlobPatternExchange("*");

builder.Services
	.AddProfileViewer()
	.AddHttpLogging(logging =>
	{
		logging.LoggingFields = HttpLoggingFields.All;
		logging.MediaTypeOptions.AddText("application/x-www-form-urlencoded");
		logging.MediaTypeOptions.AddText("text/html");
	})
	.AddHealthChecks()
	.Services
	.AddOpenTelemetryTracing(builder => builder
		.SetResourceBuilder(ResourceBuilder.CreateDefault()
			.AddEnvironmentVariableDetector())
		.AddSource("Tgs.*")
		.AddAspNetCoreInstrumentation()
		.AddHttpClientInstrumentation()
		.AddProfileViewExporter()
		.AddOtlpExporter())
	.AddOpenTelemetryMetrics(meterBuiler => meterBuiler
		.SetResourceBuilder(ResourceBuilder.CreateDefault()
			.AddEnvironmentVariableDetector())
			.AddRuntimeInstrumentation()
			.AddAspNetCoreInstrumentation()
			.AddOtlpExporter())
	.Configure<AspNetCoreInstrumentationOptions>(options => options.Filter =
		httpContext => !httpContext.Request.Path.HasValue
			|| !httpContext.Request.Path.StartsWithSegments("/swagger")
			&& !httpContext.Request.Path.StartsWithSegments("/healthz")
			&& !httpContext.Request.Path.StartsWithSegments("/metrics")
			&& !new[] { "html", "ico", "png", "jpg" }.Any(
				ext => httpContext.Request.Path.Value.EndsWith($".{ext}")));

var app = builder.Build();

app.UseHttpLogging();
app.MapControllers();
app.MapHealthChecks("/healthz");
app.MapProfileViewer();

app.Use(async (context, next) =>
{
	await next().ConfigureAwait(false);

	var logger = context.RequestServices.GetService<ILoggerFactory>()
		?.CreateLogger("Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware");

	if (logger?.IsEnabled(LogLevel.Information) == true)
	{
		using var stream = new StreamReader(context.Request.Body);

		_ = await stream.ReadToEndAsync().ConfigureAwait(false);
	}
});

app.Run();
