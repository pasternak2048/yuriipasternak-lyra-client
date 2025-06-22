using LYRA.Client.Constants;
using LYRA.Security.Enums;

namespace LYRA.Client.Models
{
    /// <summary>
    /// Represents a signed metadata structure used for secure communication between services,
    /// including information required for signature verification by LYRA.Server.
    /// </summary>
    public class LyraMetadata
    {
        /// <summary>
        /// The system name of the initiating touchpoint (e.g., "gateway@bcorp").
        /// </summary>
        public string Caller { get; init; } = default!;

        /// <summary>
        /// The system name of the receiving touchpoint (e.g., "billing@acorp").
        /// </summary>
        public string Target { get; init; } = default!;

        /// <summary>
        /// Logical method or action name (e.g., "POST", "PUBLISH", "CALL", "SET").
        /// </summary>
        public string Method { get; init; } = default!;

        /// <summary>
        /// Path, topic, or operation identifier depending on the context.
        /// Examples: "/api/orders", "order.created", "CacheService.GetUser".
        /// </summary>
        public string Path { get; init; } = default!;

        /// <summary>
        /// The original payload that was sent in the request (e.g., serialized JSON).
        /// Optional and not required for all communication types.
        /// </summary>
        public string? Payload { get; init; }

        /// <summary>
        /// Base64-encoded SHA-512 hash of the payload content.
        /// Used to ensure the payload integrity during signature verification.
        /// </summary>
        public string PayloadHash { get; init; } = default!;

        /// <summary>
        /// UTC timestamp (ISO 8601 format) indicating when the signature was generated.
        /// </summary>
        public string Timestamp { get; init; } = default!;

        /// <summary>
        /// Access context indicating the type of communication (e.g., Http, Event, Grpc, Cache).
        /// </summary>
        public AccessContext Context { get; init; }

        /// <summary>
        /// The computed Base64-encoded signature for this metadata.
        /// </summary>
        public string Signature { get; init; } = default!;

        /// <summary>
        /// Converts this metadata object into a dictionary of LYRA-compliant headers.
        /// Useful for injecting into outgoing HTTP requests or message headers.
        /// </summary>
        /// <returns>Dictionary containing all metadata values as LYRA headers.</returns>
        public IDictionary<string, string> ToHeaders() => new Dictionary<string, string>
        {
            [LyraHeaderNames.Caller] = Caller,
            [LyraHeaderNames.Target] = Target,
            [LyraHeaderNames.Method] = Method,
            [LyraHeaderNames.Path] = Path,
            [LyraHeaderNames.Payload] = Payload ?? string.Empty,
            [LyraHeaderNames.PayloadHash] = PayloadHash,
            [LyraHeaderNames.Timestamp] = Timestamp,
            [LyraHeaderNames.Context] = Context.ToString(),
            [LyraHeaderNames.Signature] = Signature
        };
    }
}
