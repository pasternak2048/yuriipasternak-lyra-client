using LYRA.Client.Configuration;
using LYRA.Client.Constants;
using LYRA.Security.Enums;
using LYRA.Security.Models.Verify;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace LYRA.Client.Middleware
{
    /// <summary>
    /// Middleware that intercepts incoming HTTP requests and verifies their LYRA signature
    /// by sending a verification request to the configured LYRA.Server instance.
    /// </summary>
    public class LyraVerificationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LyraReceiverOptions _options;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes the middleware with the next delegate, LYRA options, and HTTP client factory.
        /// </summary>
        public LyraVerificationMiddleware(
            RequestDelegate next,
            IOptions<LyraReceiverOptions> options,
            IHttpClientFactory httpClientFactory)
        {
            _next = next;
            _options = options.Value;
            _httpClient = httpClientFactory.CreateClient(nameof(LyraVerificationMiddleware));
        }

        /// <summary>
        /// Intercepts the request, extracts LYRA headers, sends a verification request to LYRA.Server,
        /// and proceeds or blocks based on the result.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            var headers = context.Request.Headers;

            // Check required headers
            if (!headers.ContainsKey(LyraHeaderNames.Caller) ||
                !headers.ContainsKey(LyraHeaderNames.Target) ||
                !headers.ContainsKey(LyraHeaderNames.Method) ||
                !headers.ContainsKey(LyraHeaderNames.Path) ||
                !headers.ContainsKey(LyraHeaderNames.PayloadHash) ||
                !headers.ContainsKey(LyraHeaderNames.Timestamp) ||
                !headers.ContainsKey(LyraHeaderNames.Context) ||
                !headers.ContainsKey(LyraHeaderNames.Signature))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Missing required LYRA signature headers.");
                return;
            }

            // Attempt to parse context enum
            if (!Enum.TryParse<AccessContext>(headers[LyraHeaderNames.Context].ToString(), out var contextEnum))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid LYRA context header.");
                return;
            }

            // Construct verification request
            var verifyRequest = new VerifyRequest
            {
                Caller = headers[LyraHeaderNames.Caller].ToString(),
                Target = headers[LyraHeaderNames.Target].ToString(),
                Method = headers[LyraHeaderNames.Method].ToString(),
                Path = headers[LyraHeaderNames.Path].ToString(),
                Payload = headers[LyraHeaderNames.Payload].ToString(),
                PayloadHash = headers[LyraHeaderNames.PayloadHash].ToString(),
                Timestamp = headers[LyraHeaderNames.Timestamp].ToString(),
                Context = contextEnum,
                Signature = headers[LyraHeaderNames.Signature].ToString()
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    $"{_options.LyraServerHost}/api/verify",
                    verifyRequest);

                if (!response.IsSuccessStatusCode)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync($"LYRA verification failed: {response.StatusCode}");
                    return;
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status502BadGateway;
                await context.Response.WriteAsync($"LYRA verification error: {ex.Message}");
                return;
            }

            // Continue to next middleware
            await _next(context);
        }
    }
}
