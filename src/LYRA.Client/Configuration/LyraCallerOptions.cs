using LYRA.Client.Models;
using LYRA.Security.Enums;

namespace LYRA.Client.Configuration
{
    /// <summary>
    /// Configuration options for the LYRA Caller, allowing registration of multiple trusted touchpoints
    /// that can sign outgoing requests using different secrets and contexts.
    /// </summary>
    public class LyraCallerOptions
    {
        /// <summary>
        /// A collection of touchpoints that represent the caller's identities, each with its own secret,
        /// context, and signature algorithm. One of them is selected dynamically during signing.
        /// </summary>
        public List<LyraTouchpoint> Touchpoints { get; set; } = new();
    }
}
