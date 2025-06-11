namespace LYRA.Client.Configuration
{
    /// <summary>
    /// Configuration options for LYRA Receiver mode (used for verifying incoming signed requests).
    /// </summary>
    public class LyraReceiverOptions
    {
        /// <summary>
        /// Base URL of the LYRA.Server instance (e.g., "https://lyra.company.com").
        /// This is used by the receiver to send <see cref="VerifyRequest"/>s for validation.
        /// The URL should not include a trailing slash.
        /// </summary>
        public string LyraServerHost { get; set; } = null!;
    }
}
