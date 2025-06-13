using LYRA.Client.Constants;
using LYRA.Client.Interfaces;
using LYRA.Security.Enums;
using LYRA.Security.Models.Verify;
using Microsoft.AspNetCore.Http;

namespace LYRA.Client.Middleware
{
    /// <summary>
    /// Middleware that intercepts incoming HTTP requests and verifies their LYRA signature
    /// by sending a verification request to the configured LYRA.Server instance.
    /// </summary>
    public class LyraVerificationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILyraReceiver _receiver;

        /// <summary>
        /// Initializes the middleware with the next delegate and LYRA verification service.
        /// </summary>
        public LyraVerificationMiddleware(RequestDelegate next, ILyraReceiver receiver)
        {
            _next = next;
            _receiver = receiver;
        }

        /// <summary>
        /// Intercepts the request, extracts LYRA headers, sends a verification request to LYRA.Server,
        /// and proceeds or blocks based on the result.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            var headers = context.Request.Headers;

            // Required LYRA headers
            var requiredHeaders = new[]
            {
                LyraHeaderNames.Caller,
                LyraHeaderNames.Target,
                LyraHeaderNames.Method,
                LyraHeaderNames.Path,
                LyraHeaderNames.PayloadHash,
                LyraHeaderNames.Timestamp,
                LyraHeaderNames.Context,
                LyraHeaderNames.Signature
            };

            // Check missing headers
            var missing = requiredHeaders.Where(h => !headers.ContainsKey(h)).ToList();
            if (missing.Any())
            {
                await Fail(context, StatusCodes.Status400BadRequest,
                    $"Missing required LYRA headers: {string.Join(", ", missing)}");
                return;
            }

            // Parse AccessContext
            if (!Enum.TryParse<AccessContext>(headers[LyraHeaderNames.Context].ToString(), ignoreCase: true, out var contextEnum))
            {
                await Fail(context, StatusCodes.Status400BadRequest, "Invalid LYRA context header.");
                return;
            }

            // Construct VerifyRequest
            var verifyRequest = new VerifyRequest
            {
                Caller = headers[LyraHeaderNames.Caller]!,
                Target = headers[LyraHeaderNames.Target]!,
                Method = headers[LyraHeaderNames.Method]!,
                Path = headers[LyraHeaderNames.Path]!,
                Payload = headers.TryGetValue(LyraHeaderNames.Payload, out var payload) ? payload.ToString() : null,
                PayloadHash = headers[LyraHeaderNames.PayloadHash]!,
                Timestamp = headers[LyraHeaderNames.Timestamp]!,
                Context = contextEnum,
                Signature = headers[LyraHeaderNames.Signature]!
            };

            // Verify
            var result = await _receiver.VerifyAsync(verifyRequest);

            if (result is not { IsSuccess: true })
            {
                await Fail(context, result?.StatusCode ?? StatusCodes.Status403Forbidden,
                    result?.ErrorMessage ?? "LYRA verification failed.");
                return;
            }

            await _next(context);
        }

        private static async Task Fail(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(message);
        }
    }
}
