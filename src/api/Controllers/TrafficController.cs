using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Traffic.Shared;

namespace Traffic.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class TrafficController : ControllerBase
    {
        private readonly TrafficProxy _proxy;

        public TrafficController(TrafficProxy proxy)
        {
            _proxy = proxy;
        }

        [Route("")]
        [HttpGet]
        [OutputCache(PolicyName = "Expire30")]
        public async Task<TrafficDto> Get()
        {                       
            return await _proxy.GetTrafficAsync();
        }
    }
}
