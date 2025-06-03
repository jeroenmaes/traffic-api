using Microsoft.Extensions.DependencyInjection;

namespace Traffic.Client
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the Traffic API client services to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="baseUrl">The base URL of the Traffic API</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddTrafficClient(this IServiceCollection services, string baseUrl)
        {
            services.AddHttpClient<ITrafficClient, TrafficClient>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return services;
        }
    }
}
