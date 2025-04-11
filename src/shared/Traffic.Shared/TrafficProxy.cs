using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Traffic.API;

namespace Traffic.Shared
{
    public class TrafficProxy
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TrafficProxy> _logger;

        public TrafficProxy(ILogger<TrafficProxy> logger, HttpClient httpClient) 
        {
            _logger = logger;
            _httpClient = httpClient;
        }
        public async Task<TrafficDto> GetTraffic()
        {
            var traffic = new TrafficDto();
            string url = "https://www.verkeerscentrum.be/filebarometer?var=" + DateTime.UtcNow.Ticks;
            try
            {
                var response = await _httpClient.GetStringAsync(url);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                // Example XPath - Adjust based on actual HTML structure
                var trafficNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"block-vvctotaltrafficcongestion\"]/div/span[2]/a");
                if (trafficNode != null)
                {
                    string rawData = trafficNode.InnerHtml.Trim();

                    traffic.Amount = double.Parse(rawData.Split("km")[0]);
                    traffic.Trend = rawData.Split("km")[1].Replace("(", "").Replace(")", "");
                    _logger.LogInformation("Current Traffic Info: " + traffic.Amount);


                    traffic.TimestampUpdated = DateTime.Parse(DateTime.UtcNow.ToString("g"));
                }
                else
                {
                    _logger.LogInformation("Traffic information not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving Traffic Info: " + ex.Message);
            }

            return traffic;
        }
    }
}
