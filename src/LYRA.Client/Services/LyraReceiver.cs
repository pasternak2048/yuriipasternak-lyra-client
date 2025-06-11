using LYRA.Client.Configuration;
using LYRA.Client.Interfaces;
using LYRA.Client.Middleware;
using LYRA.Security.Models.Verify;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace LYRA.Client.Services
{
    /// <summary>
    /// Default implementation of <see cref="ILyraReceiver"/> that sends signed request verification requests to LYRA.Server.
    /// </summary>
    internal class LyraReceiver : ILyraReceiver
    {
        private readonly LyraReceiverOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="LyraReceiver"/> class.
        /// </summary>
        /// <param name="options">The receiver configuration options.</param>
        /// <param name="httpClientFactory">Factory used to create HTTP clients.</param>
        internal LyraReceiver(
            IOptions<LyraReceiverOptions> options,
            IHttpClientFactory httpClientFactory)
        {
            _options = options.Value;
            _httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc />
        public async Task<VerifyResponse?> VerifyAsync(VerifyRequest request)
        {
            var client = _httpClientFactory.CreateClient(nameof(LyraVerificationMiddleware));

            var response = await client.PostAsJsonAsync($"{_options.LyraServerHost.TrimEnd('/')}/api/verify", request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<VerifyResponse>();
        }
    }
}
