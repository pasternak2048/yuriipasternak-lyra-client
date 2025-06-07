using LYRA.Client.Configuration;
using LYRA.Client.Interfaces;
using LYRA.Security.Models.Verify;
using LYRA.Security.Signature;
using LYRA.Security.Utilities.Security;
using Microsoft.Extensions.Options;

namespace LYRA.Client.Services
{
    /// <summary>
    /// Default implementation of ILyraCaller that generates LYRA signature headers for outgoing requests.
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

        public async Task<IDictionary<string, string>> GenerateSignatureHeadersAsync(string method, string path, string? payload = null)
        {
            var headers = new Dictionary<string, string>();

            // Compute payload hash
            string payloadHash = payload != null
                ? EncryptionHelper.ComputeSha512(payload)
                : string.Empty;

            // Generate timestamp
            string timestamp = DateTimeOffset.UtcNow.ToString("O");

            // Create VerifyRequest model
            var request = new VerifyRequest
            {
                Caller = _options.SystemName,
                Target = _options.TargetSystemName,
                Method = method,
                Path = path,
                Payload = payload,
                PayloadHash = payloadHash,
                Timestamp = timestamp,
                Context = _options.Context
            };

            // Generate canonical string to sign
            string stringToSign = _stringBuilder.BuildStringToSign(request);

            // Sign the string
            string signature = EncryptionHelper.ComputeHmacSha512(stringToSign, _options.Secret);

            request.Signature = signature;

            // Add headers
            headers["caller"] = request.Caller;
            headers["target"] = request.Target;
            headers["method"] = request.Method;
            headers["path"] = request.Path;
            headers["payload"] = request.Payload ?? string.Empty;
            headers["payloadHash"] = request.PayloadHash;
            headers["timestamp"] = request.Timestamp;
            headers["context"] = request.Context.ToString();
            headers["signature"] = request.Signature;

            return await Task.FromResult(headers);
        }
    }
}
