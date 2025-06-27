
using SlimMessageBus;
using Traffic.API.Data;

namespace Traffic.API.Monitors
{
    public class TrafficMonitor : BackgroundService
    {
        private readonly ILogger<TrafficMonitor> _logger;       
        private readonly TrafficProxy _proxy;
        private TrafficDto? previousTraffic;
        private readonly IMessageBus _bus;

        public TrafficMonitor(ILogger<TrafficMonitor> logger, TrafficProxy proxy, IMessageBus bus)
        {
            _logger = logger;
            _proxy = proxy;
            previousTraffic = new TrafficDto();
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    
                    // Retrieve the current traffic data
                    var currentTraffic = await _proxy.GetTrafficAsync("BE", "FL");                    

                    // Compare the previous with the current traffic data
                    if (previousTraffic.Amount != currentTraffic.Amount)
                    {
                        // Publish an event if there is a change
                        _logger.LogInformation("Traffic data has changed. Publishing event...");

                        await _bus.Publish(new TrafficChangedV1());
                    }
                    else
                    {
                        _logger.LogInformation("Traffic data is unchanged.");
                    }

                    // Update the previous traffic data
                    previousTraffic = currentTraffic;
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in TrafficMonitor: {ex.Message}", ex);
                    //throw;
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Poll every 30 sec
            }
        }
    }
}
