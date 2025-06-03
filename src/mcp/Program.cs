using Traffic.Shared;

namespace Traffic.MCP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpClient("verkeerscentrum");
            builder.Services.AddTransient<TrafficProxy, TrafficProxy>();
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
