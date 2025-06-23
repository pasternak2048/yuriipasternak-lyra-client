using LYRA.Client.Interfaces;
using System.Text;
using System.Text.Json;

namespace LYRA.Client.Signers.Http
{
    /// <summary>
    /// Default implementation of <see cref="ILyraSignedHttpClient"/> that sends HTTP requests
    /// with automatically attached LYRA signature headers based on configured touchpoints.
    /// </summary>
    public class LyraSignedHttpClient : ILyraSignedHttpClient
    {
        private readonly HttpClient _http;
        private readonly ILyraCaller _lyra;

        public LyraSignedHttpClient(HttpClient http, ILyraCaller lyra)
        {
            _http = http;
            _lyra = lyra;
        }

        /// <inheritdoc/>
        public async Task<TResponse?> SendAsync<TRequest, TResponse>(
            HttpMethod method,
            string path,
            string targetSystem,
            string callerSystem,
            TRequest body,
            CancellationToken cancellationToken = default)
        {
            var payload = JsonSerializer.Serialize(body);

            var metadata = _lyra.GenerateSignedMetadata(
                method: method.Method,
                path: path,
                targetSystemName: targetSystem,
                payload: payload,
                callerSystemName: callerSystem);

            var request = new HttpRequestMessage(method, path)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

            foreach (var (key, value) in metadata.ToHeaders())
                request.Headers.Add(key, value);

            var response = await _http.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception($"Signed request failed: {(int)response.StatusCode} {response.ReasonPhrase} | {message}");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<TResponse>(content);
        }
    }
}
