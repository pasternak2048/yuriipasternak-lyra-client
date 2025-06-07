namespace LYRA.Client.Interfaces
{
    /// <summary>
    /// Defines a service responsible for generating LYRA-compliant signature headers for outgoing requests.
    /// </summary>
    public interface ILyraCaller
    {
        /// <summary>
        /// Generates LYRA signature headers for an outgoing HTTP request.
        /// </summary>
        /// <param name="method">HTTP method (e.g. GET, POST).</param>
        /// <param name="path">Request path (e.g. /api/orders).</param>
        /// <param name="payload">Optional raw request body.</param>
        /// <returns>A dictionary of LYRA headers to include in the HTTP request.</returns>
        Task<IDictionary<string, string>> GenerateSignatureHeadersAsync(string method, string path, string? payload = null);
    }
}
