namespace Traffic.API.Data
{
    public class TrafficDto
    {
        public decimal Amount { get; set; } = -1;
        public string Unit { get; set; } = "Kilometers";
        public DateTime TimestampUpdated { get; set; } = DateTime.Parse("1900-01-01T01:00:00");
        public string Source { get; set; } = "verkeerscentrum.be";
        public string Trend { get; set; }
    }
}
