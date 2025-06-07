using LYRA.Client.Configuration;
using LYRA.Client.Interfaces;
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
    }
}
