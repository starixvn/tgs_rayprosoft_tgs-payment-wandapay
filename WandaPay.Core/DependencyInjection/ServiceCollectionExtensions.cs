using System.Reflection;
using Tgs.Payment.WandaPay;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddWandaPay(
		this IServiceCollection services,
		Action<WandaPayOptions> configure,
		Action<IHttpClientBuilder> wandaPayClientConfigure)
	{
		wandaPayClientConfigure(services.AddHttpClient<IWandaPayClient, WandaPayClient>());

		return services
			.AddOptions<WandaPayOptions>()
			.Configure(configure)
			.Services
			.RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
			.AddSingleton<WandaPayHasher>();
	}
}
