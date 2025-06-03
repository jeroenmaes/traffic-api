using ModelContextProtocol.Server;
using System.ComponentModel;
using Traffic.APIClient;

namespace Traffic.MCP
{
    [McpServerToolType]
    public class TrafficTool
    {
        private readonly ITrafficClient _trafficClient;

        public TrafficTool(ITrafficClient trafficClient)
        {
            _trafficClient = trafficClient;
        }

        [McpServerTool, Description("Get the current traffic for the Flemish region in Belgium")]
        public async Task<string> GetCurrentTraffic()
        { 
            var traffic = await _trafficClient.GetTrafficAsync();
            return $"The current amount of traffic is '{traffic.Amount} {traffic.Unit}'. The trend is '{traffic.Trend}'. This data was retrieved on '{traffic.TimestampUpdated.ToLocalTime()}' from '{traffic.Source}'.";
        }
        
    }
}