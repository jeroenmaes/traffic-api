using System.Collections.Concurrent;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Traffic.API.Data
{
    public class IrmKmiApiException : Exception
    {
        public IrmKmiApiException(string message, Exception inner = null) : base(message, inner) { }
    }

    public class IrmKmiApiCommunicationException : IrmKmiApiException
    {
        public IrmKmiApiCommunicationException(string message, Exception inner = null) : base(message, inner) { }
    }

    public class IrmKmiApiParametersException : IrmKmiApiException
    {
        public IrmKmiApiParametersException(string message, Exception inner = null) : base(message, inner) { }
    }

    public class ForecastProxy
    {
        private const int COORD_DECIMALS = 6;
        private const int CACHE_MAX_AGE_SECONDS = 60 * 60 * 2;
        private const string USER_AGENT = "YOUR_USER_AGENT_STRING";
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://app.meteo.be/services/appv4/";

        private static readonly ConcurrentDictionary<string, (string ETag, ForecastDto Response, DateTime Timestamp)> _cache = new();

        public ForecastProxy(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(USER_AGENT);
        }

        public async Task<ForecastDto> GetForecastsCoordAsync(string cordlat, string cordlong)
        {
            if (string.IsNullOrWhiteSpace(cordlat) || string.IsNullOrWhiteSpace(cordlong))
            {
                throw new IrmKmiApiParametersException("Coordinates cannot be null or empty.");
            }

            var intLat = Math.Round(Decimal.Parse(cordlat), COORD_DECIMALS);
            var intLong = Math.Round(Decimal.Parse(cordlong), COORD_DECIMALS);

            var parameters = new Dictionary<string, string>
            {
                ["s"] = "getForecasts",
                ["k"] = GenerateApiKey("getForecasts"),
                ["lat"] = intLat.ToString(CultureInfo.InvariantCulture),
                ["long"] = intLong.ToString(CultureInfo.InvariantCulture)
            };

            return await ApiWrapperAsync(parameters);
        }

        private static string GenerateApiKey(string methodName)
        {
            var input = $"r9EnW374jkJ9acc;{methodName};{DateTime.Now:dd/MM/yyyy}";
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private async Task<ForecastDto> ApiWrapperAsync(
            Dictionary<string, string> parameters,
            string baseUrl = null,
            string path = "",
            HttpMethod method = null,
            object data = null,
            Dictionary<string, string> headers = null)
        {
            string url = $"{(baseUrl ?? _baseUrl)}{path}";

            AddHeaders(headers);

            if (_cache.TryGetValue(url, out var cached) && (DateTime.UtcNow - cached.Timestamp).TotalSeconds < CACHE_MAX_AGE_SECONDS)
            {
                _httpClient.DefaultRequestHeaders.IfNoneMatch.Clear();
                _httpClient.DefaultRequestHeaders.IfNoneMatch.Add(new EntityTagHeaderValue($"\"{cached.ETag}\""));
            }

            try
            {
                var uriBuilder = new UriBuilder(url);
                if (parameters.Count > 0)
                {
                    var query = new FormUrlEncodedContent(parameters);
                    uriBuilder.Query = await query.ReadAsStringAsync();
                }

                var request = new HttpRequestMessage(method ?? HttpMethod.Get, uriBuilder.Uri);

                if (data != null)
                {
                    var json = JsonSerializer.Serialize(data);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
                {
                    return cached.Response;
                }

                response.EnsureSuccessStatusCode();

                var jsonResp = await response.Content.ReadFromJsonAsync<ForecastDto>();

                if (response.Headers.ETag != null)
                {
                    _cache[url] = (response.Headers.ETag.Tag.Trim('"'), jsonResp, DateTime.UtcNow);
                }

                return jsonResp;
            }
            catch (TaskCanceledException ex)
            {
                throw new IrmKmiApiCommunicationException("Timeout error fetching information", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new IrmKmiApiCommunicationException("Error fetching information", ex);
            }
            catch (Exception ex)
            {
                throw new IrmKmiApiException($"Unexpected error occurred: {ex.Message}", ex);
            }
        }

        private void AddHeaders(Dictionary<string, string> headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    _httpClient.DefaultRequestHeaders.Remove(header.Key);
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
        }
        public void ExpireCache()
        {
            var now = DateTime.UtcNow;
            int expiredCount = 0;
            foreach (var key in _cache.Keys)
            {
                if ((now - _cache[key].Timestamp).TotalSeconds > CACHE_MAX_AGE_SECONDS)
                {
                    _cache.TryRemove(key, out _);
                    expiredCount++;
                }
            }

            Console.WriteLine($"Expired {expiredCount} elements from API cache.");
        }
    }
}
