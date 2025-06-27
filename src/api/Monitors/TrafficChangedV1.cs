
using Traffic.API.Data;

namespace Traffic.API.Monitors
{
    public class TrafficChangedV1
    {

        public decimal CurrentAmount { get; set; } = -1;
        public decimal PreviousAmount { get; set; } = -1;
        public string Unit { get; set; } = "Kilometers";
        public DateTime TimestampUpdated { get; set; } = DateTime.Parse("1900-01-01T01:00:00");
        public string Source { get; set; } = "verkeerscentrum.be";
        public string Trend { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
    }

}