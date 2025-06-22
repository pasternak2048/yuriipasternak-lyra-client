using LYRA.Client.Configuration;
using LYRA.Client.Interfaces;
using LYRA.Client.Models;
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
        public LyraMetadata GenerateSignedMetadata(
            string method,
            string path,
            string targetSystemName,
            string? payload = null,
            string? callerSystemName = null)
        {
            var touchpoint = !string.IsNullOrWhiteSpace(callerSystemName)
                ? _options.Touchpoints.FirstOrDefault(t => t.SystemName == callerSystemName)
                : _options.Touchpoints.FirstOrDefault();

            if (touchpoint is null)
                throw new InvalidOperationException("No suitable touchpoint found for LYRA signature generation.");

            var payloadHash = !string.IsNullOrEmpty(payload)
                ? EncryptionHelper.ComputeSha512(payload)
                : string.Empty;

            var timestamp = DateTime.UtcNow.ToString("O");

            var stringToSign = _stringBuilder.BuildStringToSign(
                caller: touchpoint.SystemName,
                target: targetSystemName,
                method: method,
                path: path,
                payloadHash: payloadHash,
                timestamp: timestamp
            );

            var signature = EncryptionHelper.ComputeHmacSha512(stringToSign, touchpoint.Secret);

            return new LyraMetadata
            {
                Caller = touchpoint.SystemName,
                Target = targetSystemName,
                Method = method,
                Path = path,
                Payload = payload,
                PayloadHash = payloadHash,
                Timestamp = timestamp,
                Context = touchpoint.Context,
                Signature = signature
            };
        }
    }
}