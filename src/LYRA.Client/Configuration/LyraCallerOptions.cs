using LYRA.Security.Enums;

namespace LYRA.Client.Configuration
{
    /// <summary>
    /// Configuration options for the LYRA caller (used to sign outgoing requests).
    /// </summary>
    public class LyraCallerOptions
    {
        /// <summary>
        /// Unique system name of the caller (e.g., "gateway@bcorp").
        /// </summary>
        public string SystemName { get; set; } = null!;

        /// <summary>
        /// Unique system name of the expected target (e.g., "billing@acorp").
        /// </summary>
        public string TargetSystemName { get; set; } = null!;

        /// <summary>
        /// Secret key used for signing requests (HMAC or other supported method).
        /// </summary>
        public string Secret { get; set; } = null!;

        /// <summary>
        /// Context in which the request is made (Http, Event, Cache, etc.).
        /// </summary>
        public AccessContext Context { get; set; } = AccessContext.Http;
    }
}
