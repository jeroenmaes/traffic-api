
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Traffic.Shared;

namespace Traffic.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
                        
            builder.Services.AddControllers().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault; // ignore default values, including null for nullable types        
                o.JsonSerializerOptions.WriteIndented = true;
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                o.JsonSerializerOptions.AllowTrailingCommas = true;
                o.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

            builder.Services.AddOpenApi();
            
            builder.Services.AddHttpClient("verkeerscentrum");
            builder.Services.AddTransient<TrafficProxy, TrafficProxy>();
            builder.Services.AddTransient<ForecastProxy, ForecastProxy>();

            builder.Services.AddHealthChecks();
            builder.Services.AddMemoryCache();

            builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            
            builder.Services.AddOutputCache(options =>
            {
                options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromMinutes(10)));
                options.AddPolicy("Expire30", builder => builder.Expire(TimeSpan.FromSeconds(30)));
            });
            
            var app = builder.Build();

            app.UseOutputCache();

            app.MapOpenApi().CacheOutput();

            app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "v1"); });

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.Run();
        }
    }
}
