using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Logging;
using Traffic.API.Data;

namespace Traffic.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("v1/[controller]")]    
    public class TrafficController : ControllerBase
    {
        private readonly TrafficProxy _proxy;
        private readonly ILogger<TrafficController> _logger;

        public TrafficController(TrafficProxy proxy, ILogger<TrafficController> logger)
        {
            _proxy = proxy;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger = logger;
        }

        [Route("")]
        [HttpGet]
        [OutputCache(PolicyName = "Expire30")]
        public async Task<TrafficDto> Get(string country = "BE", string region = "FL")
        {                       
            var traffic = await _proxy.GetTrafficAsync(country, region);

            _logger.LogInformation("Current Traffic Info: {Amount} km, Trend: {Trend}", traffic.Amount, traffic.Trend);

            return traffic;
        }
    }
}
