namespace LYRA.Client.Interfaces
{
    /// <summary>
    /// Defines a client capable of sending outbound HTTP requests that are signed
    /// using LYRA metadata for secure cross-service communication.
    /// </summary>
    public interface ILyraSignedHttpClient
    {
        /// <summary>
        /// Sends an HTTP request with the provided method, path, and body,
        /// automatically attaching LYRA-compliant signature headers.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request payload (body).</typeparam>
        /// <typeparam name="TResponse">The expected type of the response payload.</typeparam>
        /// <param name="method">The HTTP method to use (e.g., GET, POST, PUT, DELETE).</param>
        /// <param name="path">The relative URI path for the request (e.g., "/api/orders").</param>
        /// <param name="targetSystem">
        /// The system name of the intended target (used to build signature metadata).
        /// For example, "orders@gloria".
        /// </param>
        /// <param name="callerSystem">
        /// The system name of the caller (signing identity).
        /// Must match a configured LYRA touchpoint.
        /// </param>
        /// <param name="body">The request body to send, typically serialized as JSON.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>
        /// A deserialized response of type <typeparamref name="TResponse"/>,
        /// or <c>null</c> if no content is returned.
        /// </returns>
        Task<TResponse?> SendAsync<TRequest, TResponse>(
            HttpMethod method,
            string path,
            string targetSystem,
            string callerSystem,
            TRequest body,
            CancellationToken cancellationToken = default);
    }
}
