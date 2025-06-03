using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Traffic.Client
{
    public class TrafficClient : ITrafficClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TrafficClient>? _logger;

        public TrafficClient(HttpClient httpClient, ILogger<TrafficClient>? logger = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        /// <summary>
        /// Gets the current traffic information from the API
        /// </summary>
        /// <returns>A TrafficDto containing traffic information</returns>
        public async Task<TrafficDto> GetTrafficAsync()
        {
            try
            {
                _logger?.LogInformation("Requesting traffic data from API");
                var response = await _httpClient.GetAsync("v1/traffic");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var trafficData = JsonSerializer.Deserialize<TrafficDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (trafficData != null)
                {
                    _logger?.LogInformation("Retrieved traffic data: {Amount} {Unit}, Trend: {Trend}", 
                        trafficData.Amount, trafficData.Unit, trafficData.Trend);
                    return trafficData;
                }
                
                _logger?.LogWarning("Received null traffic data from API");
                return new TrafficDto();
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, "Error requesting traffic data: {Message}", ex.Message);
                return new TrafficDto { Trend = $"Error: {ex.Message}" };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error retrieving traffic data: {Message}", ex.Message);
                return new TrafficDto { Trend = "Error: Unable to retrieve traffic data" };
            }
        }
    }
}
