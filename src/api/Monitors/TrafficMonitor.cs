
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
                        var changedEvent = new TrafficChangedV1()
                        {
                            CurrentAmount = currentTraffic.Amount,
                            PreviousAmount = previousTraffic.Amount,
                            Unit = currentTraffic.Unit,
                            TimestampUpdated = currentTraffic.TimestampUpdated,
                            Source = currentTraffic.Source,
                            Trend = currentTraffic.Trend,
                            Country = currentTraffic.Country,
                            Region = currentTraffic.Region
                        };
                        _logger.LogInformation("Traffic Changed: {CurrentAmount} km, Previous Amount: {PreviousAmount} km, Trend: {Trend}", changedEvent.CurrentAmount, changedEvent.PreviousAmount, changedEvent.Trend);
                        await _bus.Publish(changedEvent);
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
