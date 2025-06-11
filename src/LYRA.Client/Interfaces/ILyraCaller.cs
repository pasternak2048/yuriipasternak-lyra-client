namespace LYRA.Client.Interfaces
{
    /// <summary>
    /// Defines a service responsible for generating LYRA-compliant signature headers for outgoing requests.
    /// Supports multiple caller touchpoints with individual configuration.
    /// </summary>
    public interface ILyraCaller
    {
        /// <summary>
        /// Generates LYRA-compliant signature headers for an outgoing request based on the provided parameters.
        /// </summary>
        /// <param name="method">
        /// Logical operation type, depending on the request context.
        /// Examples:
        /// - "POST" (for HTTP),
        /// - "publish" (for Event),
        /// - "SET" or "GET" (for Cache),
        /// - "CALL" (for gRPC),
        /// - "INVOKE" (for internal).
        /// </param>
        /// <param name="path">
        /// Resource or operation identifier, such as:
        /// - HTTP path (e.g., "/api/orders"),
        /// - Event topic (e.g., "order.created"),
        /// - Cache key (e.g., "user:123"),
        /// - gRPC method (e.g., "OrderService.CreateOrder").
        /// </param>
        /// <param name="targetSystemName">The system name of the intended recipient (e.g., "billing@acorp").</param>
        /// <param name="payload">Optional raw payload body (e.g., JSON string).</param>
        /// <param name="callerSystemName">
        /// Optional system name of the caller. If not specified, the first configured touchpoint will be used.
        /// </param>
        /// <returns>
        /// A dictionary of LYRA signature headers:
        /// `caller`, `target`, `method`, `path`, `payload`, `payloadHash`, `timestamp`, `context`, and `signature`,
        /// which should be included in the outgoing request.
        /// </returns>
        Task<IDictionary<string, string>> GenerateSignatureHeadersAsync(
            string method,
            string path,
            string targetSystemName,
            string? payload = null,
            string? callerSystemName = null);
    }
}
