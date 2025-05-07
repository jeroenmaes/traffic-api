using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Traffic.Shared;

namespace Traffic.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/[controller]")]    
    public class ForecastController : ControllerBase
    {
        private readonly ForecastProxy _proxy;

        public ForecastController(ForecastProxy proxy)
        {
            _proxy = proxy;
        }

        [Route("")]
        [HttpGet]
        [OutputCache(PolicyName = "Expire30")]
        public async Task<ForecastDto> Get(string cordlat = "51,1558839", string cordlong = "4,154441")
        {   
            return await _proxy.GetForecastsCoordAsync(cordlat, cordlong);
        }
    }
}
