using LYRA.Client.Models;

namespace LYRA.Client.Interfaces
{
    /// <summary>
    /// Defines a service responsible for generating LYRA-compliant signed metadata
    /// for secure outgoing communication across services (HTTP, events, gRPC, cache).
    /// </summary>
    public interface ILyraCaller
    {
        /// <summary>
        /// Generates a signed metadata object for outbound operations.
        /// The result includes all fields required for LYRA verification:
        /// caller, target, method, path, payload hash, timestamp, context, and signature.
        /// </summary>
        /// <param name="method">
        /// Logical operation type depending on the communication context:
        /// <list type="bullet">
        /// <item><description>"POST", "GET", "DELETE" for HTTP</description></item>
        /// <item><description>"PUBLISH" for Events</description></item>
        /// <item><description>"SET", "GET" for Cache</description></item>
        /// <item><description>"CALL" for gRPC</description></item>
        /// <item><description>"INVOKE" for internal service calls</description></item>
        /// </list>
        /// </param>
        /// <param name="path">
        /// Operation identifier depending on context, e.g.:
        /// <list type="bullet">
        /// <item><description>HTTP path (e.g., "/api/orders")</description></item>
        /// <item><description>Event topic (e.g., "order.created")</description></item>
        /// <item><description>Cache key (e.g., "user:123")</description></item>
        /// <item><description>gRPC method (e.g., "OrderService.CreateOrder")</description></item>
        /// </list>
        /// </param>
        /// <param name="targetSystemName">System name of the intended recipient (e.g., "billing@acorp").</param>
        /// <param name="payload">Optional raw request or message body (e.g., serialized JSON).</param>
        /// <param name="callerSystemName">
        /// Optional caller system name. If not provided, the first configured touchpoint is used.
        /// </param>
        /// <returns>
        /// A <see cref="LyraMetadata"/> instance representing all data required for LYRA signature verification.
        /// It includes a <c>ToHeaders()</c> helper to convert into HTTP-style headers.
        /// </returns>
        LyraMetadata GenerateSignedMetadata(
            string method,
            string path,
            string targetSystemName,
            string? payload = null,
            string? callerSystemName = null);
    }
}
