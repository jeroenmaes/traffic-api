using Traffic.APIClient;

namespace Traffic.MCP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add the Traffic API client
            builder.Services.AddTrafficClient("http://localhost:5018");
            builder.Services.AddProblemDetails();

            builder.Services.AddMcpServer().WithHttpTransport()                                
                .WithTools<TrafficTool>();

            builder.Services.AddLogging((configure) =>
            {
                configure.SetMinimumLevel(LogLevel.Warning);
            });

            var app = builder.Build();

            app.MapMcp();
            
            app.Run();
        }
    }
}
