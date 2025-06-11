using LYRA.Security.Enums;

namespace LYRA.Client.Models
{
    /// <summary>
    /// Represents a single touchpoint identity that can be used to sign outgoing requests.
    /// Each touchpoint has its own system name, secret, context, and signature algorithm.
    /// </summary>
    public class LyraTouchpoint
    {
        /// <summary>
        /// Unique identifier of the system making the request (e.g., "gateway@bcorp").
        /// This value is sent as the "caller" in the verification request.
        /// </summary>
        public string SystemName { get; set; } = null!;

        /// <summary>
        /// Secret key used to compute the signature for this touchpoint.
        /// </summary>
        public string Secret { get; set; } = null!;

        /// <summary>
        /// The access context in which this touchpoint operates (e.g., Http, Event, Cache, etc.).
        /// </summary>
        public AccessContext Context { get; set; }

        /// <summary>
        /// Signature algorithm used by this touchpoint (e.g., HMAC, RSA).
        /// </summary>
        public SignatureType SignatureType { get; set; } = SignatureType.HMAC;
    }
}
