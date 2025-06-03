namespace Traffic.Client
{
    public interface ITrafficClient
    {
        /// <summary>
        /// Gets the current traffic information
        /// </summary>
        /// <returns>A TrafficDto containing traffic information</returns>
        Task<TrafficDto> GetTrafficAsync();
    }
}
