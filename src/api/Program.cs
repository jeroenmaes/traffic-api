
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Traffic.API.Data;
using Ktos.AspNetCore.Authentication.ApiKeyHeader;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Runtime;
using Traffic.API.Monitors;

namespace Traffic.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables();

            string? apiKey = builder.Configuration["ApiKey"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = ApiKeyHeaderAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = ApiKeyHeaderAuthenticationDefaults.AuthenticationScheme;
            }).AddApiKeyHeaderAuthentication(options => { options.ApiKey = apiKey; options.Header = "X-API-KEY"; });

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

            builder.Services.AddOpenApi(options =>
            {
                var scheme = new OpenApiSecurityScheme
                {
                    Name = "X-API-KEY",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "API Key authentication via 'X-API-KEY' header",
                    Scheme = "ApiKeyScheme",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    }
                };

                options.AddDocumentTransformer((document, context, ct) =>
                {
                    document.Components ??= new();
                    document.Components.SecuritySchemes.Add("ApiKey", scheme);
                    return Task.CompletedTask;
                });

                options.AddOperationTransformer((operation, context, ct) =>
                {
                    if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
                    {
                        operation.Security = new List<OpenApiSecurityRequirement>
                        {
                            new OpenApiSecurityRequirement
                            {
                                [scheme] = new List<string>()
                            }
                        };
                    }
                    return Task.CompletedTask;
                });
            });

            builder.Services.AddHttpClient("verkeerscentrum");
            builder.Services.AddTransient<TrafficProxy, TrafficProxy>();
            builder.Services.AddTransient<ForecastProxy, ForecastProxy>();

            builder.Services.AddHostedService<TrafficMonitor>();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapHealthChecks("/v1/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.Run();
        }
    }
}
