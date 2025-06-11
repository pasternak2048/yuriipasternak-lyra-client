using LYRA.Client.Configuration;
using LYRA.Client.Interfaces;
using LYRA.Client.Middleware;
using LYRA.Client.Services;
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
        /// Registers LYRA Caller functionality for generating signature headers.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <param name="configure">Delegate to configure LyraCallerOptions.</param>
        /// <returns>The updated IServiceCollection.</returns>
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
