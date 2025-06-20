using LYRA.Client.Constants;
using LYRA.Security.Models.Verify;

namespace LYRA.Client.Models
{
    public class SignedRequestMetadata
    {
        public VerifyRequest Request { get; init; } = default!;

        public string Signature { get; init; } = default!;

        public IDictionary<string, string> ToHeaders()
        {
            return new Dictionary<string, string>
            {
                [LyraHeaderNames.Caller] = Request.Caller,
                [LyraHeaderNames.Target] = Request.Target,
                [LyraHeaderNames.Method] = Request.Method,
                [LyraHeaderNames.Path] = Request.Path,
                [LyraHeaderNames.Payload] = Request.Payload ?? string.Empty,
                [LyraHeaderNames.PayloadHash] = Request.PayloadHash,
                [LyraHeaderNames.Timestamp] = Request.Timestamp,
                [LyraHeaderNames.Context] = Request.Context.ToString(),
                [LyraHeaderNames.Signature] = Signature
            };
        }
    }
}
