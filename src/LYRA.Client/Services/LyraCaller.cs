using LYRA.Client.Configuration;
using LYRA.Client.Interfaces;
using LYRA.Security.Models.Verify;
using LYRA.Security.Signature;
using LYRA.Security.Utilities.Security;
using Microsoft.Extensions.Options;

namespace LYRA.Client.Services
{
    /// <summary>
    /// Defines a service responsible for generating LYRA-compliant signature headers for outgoing requests.
    /// Supports multiple caller touchpoints with individual configuration.
    /// </summary>

    public class LyraCaller : ILyraCaller
    {
        private readonly LyraCallerOptions _options;
        private readonly ISignatureStringBuilder _stringBuilder;

        public LyraCaller(IOptions<LyraCallerOptions> options, ISignatureStringBuilder stringBuilder)
        {
            _options = options.Value;
            _stringBuilder = stringBuilder;
        }

        /// <inheritdoc/>
        public async Task<IDictionary<string, string>> GenerateSignatureHeadersAsync(
            string method,
            string path,
            string targetSystemName,
            string? payload = null,
            string? callerSystemName = null)
        {
            var touchpoint = !string.IsNullOrWhiteSpace(callerSystemName)
                ? _options.Touchpoints.FirstOrDefault(t => t.SystemName == callerSystemName)
                : _options.Touchpoints.FirstOrDefault();

            if (touchpoint == null)
                throw new InvalidOperationException("No suitable touchpoint found for LYRA signature generation.");

            // Compute payload hash
            var payloadHash = payload != null
                ? EncryptionHelper.ComputeSha512(payload)
                : string.Empty;

            // Generate timestamp
            var timestamp = DateTimeOffset.UtcNow.ToString("O");

            // Create VerifyRequest model
            var request = new VerifyRequest
            {
                Caller = touchpoint.SystemName,
                Target = targetSystemName,
                Method = method,
                Path = path,
                Payload = payload,
                PayloadHash = payloadHash,
                Timestamp = timestamp,
                Context = touchpoint.Context
            };

            // Generate canonical string to sign
            var stringToSign = _stringBuilder.BuildStringToSign(request);
            var signature = EncryptionHelper.ComputeHmacSha512(stringToSign, touchpoint.Secret); // TODO: support RSA etc.

            request.Signature = signature;

            // Add headers
            var headers = new Dictionary<string, string>
            {
                ["caller"] = request.Caller,
                ["target"] = request.Target,
                ["method"] = request.Method,
                ["path"] = request.Path,
                ["payload"] = request.Payload ?? string.Empty,
                ["payloadHash"] = request.PayloadHash,
                ["timestamp"] = request.Timestamp,
                ["context"] = request.Context.ToString(),
                ["signature"] = request.Signature
            };

            return await Task.FromResult(headers);
        }
    }
}
