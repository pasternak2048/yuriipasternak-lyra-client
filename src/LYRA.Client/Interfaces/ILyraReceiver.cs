using LYRA.Security.Models.Verify;

namespace LYRA.Client.Interfaces
{
    /// <summary>
    /// Defines a service that verifies incoming signed requests by delegating to LYRA.Server.
    /// </summary>
    public interface ILyraReceiver
    {
        /// <summary>
        /// Verifies the authenticity and integrity of a signed request.
        /// </summary>
        /// <param name="request">The request to verify, including signature and context details.</param>
        /// <returns>
        /// A <see cref="VerifyResponse"/> if verification succeeds, or <c>null</c> if the request is invalid or unverifiable.
        /// </returns>
        Task<VerifyResponse> VerifyAsync(VerifyRequest request);
    }
}
