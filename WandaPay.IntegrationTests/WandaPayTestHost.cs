using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;

namespace WandaPay.IntegrationTests;

internal static class WandaPayTestHost
{
	public static WebApplicationFactory<Program> Factory => new WebApplicationFactory<Program>()
		.WithWebHostBuilder(hostBuilder => hostBuilder
			.ConfigureAppConfiguration((_, configBuilder) => configBuilder.Sources.Clear())
			.ConfigureServices(services => services
				.AddSingleton<IConfigureOptions<OtlpExporterOptions>>(
					sp => new ConfigureNamedOptions<OtlpExporterOptions>(
						Options.DefaultName,
						exporterOptions => exporterOptions.TimeoutMilliseconds = 0))));
}
