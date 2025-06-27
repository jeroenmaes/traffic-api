using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Traffic.API;

namespace Traffic.API.Data
{
    public class TrafficProxy
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TrafficProxy> _logger;

        public TrafficProxy(ILogger<TrafficProxy> logger, HttpClient httpClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Retrieves the current traffic information.
        /// </summary>
        /// <returns>A <see cref="TrafficDto"/> containing the traffic information.</returns>
        public async Task<TrafficDto> GetTrafficAsync(string country, string region)
        {
            var traffic = new TrafficDto
            {
                Country = "BE",
                Region = "FL"
            };

            string url = "https://www.verkeerscentrum.be/filebarometer?var=" + DateTime.UtcNow.Ticks;
            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                var trafficNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"block-vvctotaltrafficcongestion\"]/div/span[2]/a");
                if (trafficNode != null)
                {
                    ParseTrafficData(trafficNode.InnerHtml.Trim(), traffic);
                    _logger.LogInformation("Current Traffic Info: {Amount} km, Trend: {Trend}", traffic.Amount, traffic.Trend);
                }
                else
                {
                    _logger.LogInformation("Traffic information not found.");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error retrieving Traffic Info: {Message}", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred: {Message}", ex.Message);
            }

            return traffic;
        }

        private void ParseTrafficData(string rawData, TrafficDto traffic)
        {
            try
            {
                var dataParts = rawData.Split("km");
                if (dataParts.Length > 1)
                {
                    traffic.Amount = decimal.Parse(dataParts[0]);
                    traffic.Trend = dataParts[1].Replace("(", "").Replace(")", "").Trim();
                    traffic.TimestampUpdated = DateTime.UtcNow;
                }
                else
                {
                    _logger.LogWarning("Unexpected traffic data format: {RawData}", rawData);
                }
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Error parsing traffic data: {RawData}", rawData);
            }
        }
    }
}
