using LYRA.Client.Configuration;
using LYRA.Client.Interfaces;
using LYRA.Client.Middleware;
using LYRA.Client.Services;
using LYRA.Client.Signers.Http;
using LYRA.Security.Signature;
using Microsoft.Extensions.DependencyInjection;

namespace LYRA.Client.Extensions
{
    /// <summary>
    /// Extension methods for registering LYRA caller services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers LYRA client components required to sign outgoing requests as a caller system.
        /// Supports multiple signature string builders and caller touchpoint configuration.
        /// </summary>
        /// <param name="services">The dependency injection container.</param>
        /// <param name="configure">
        /// Delegate used to configure <see cref="LyraCallerOptions"/> including touchpoints and caller identity.
        /// </param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddLyraAsCaller(
            this IServiceCollection services,
            Action<LyraCallerOptions> configure)
        {
            services.Configure(configure);

            services.AddTransient<ISignatureStringBuilder, HttpSignatureStringBuilder>();
            services.AddTransient<ISignatureStringBuilder, CacheSignatureStringBuilder>();
            services.AddSingleton<SignatureStringBuilderFactory>();

            services.AddSingleton<ILyraCaller, LyraCaller>();

            return services;
        }

        /// <summary>
        /// Registers LYRA Receiver functionality for verifying incoming requests via middleware or service.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <param name="configure">Delegate to configure LyraReceiverOptions (e.g. LYRA.Server host).</param>
        /// <returns>The updated IServiceCollection.</returns>
        public static IServiceCollection AddLyraAsReceiver(
            this IServiceCollection services,
            Action<LyraReceiverOptions> configure)
        {
            services.Configure(configure);

            services.AddHttpClient(nameof(LyraVerificationMiddleware));

            services.AddSingleton<ILyraReceiver, LyraReceiver>();

            return services;
        }
    }
}
