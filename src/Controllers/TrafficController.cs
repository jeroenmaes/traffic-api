using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Traffic.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class TrafficController : ControllerBase
    {
        
        private readonly ILogger<TrafficController> _logger;
        private readonly HttpClient _httpClient;


        public TrafficController(ILogger<TrafficController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        [Route("")]
        [HttpGet]
        [OutputCache(PolicyName = "Expire30")]
        public async Task<Traffic> Get()
        {
            string url = "https://www.verkeerscentrum.be/filebarometer?var=" + DateTime.UtcNow.Ticks;
            var traffic = new Traffic();

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
