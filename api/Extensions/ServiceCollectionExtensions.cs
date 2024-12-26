using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Shared.Settings;

namespace Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IKeycloakTokenService, KeycloakTokenService>();
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<KeycloakSettings>>().Value);

            return services;
        }


        public static void AddKeycloakSettings(this WebApplicationBuilder builder)
        {
            var keycloakSettings = builder.Configuration.GetSection("Keycloak");

            builder.Services.Configure<KeycloakSettings>(keycloakSettings);
        }

        public static void AddKeycloakAuthorization(this WebApplicationBuilder builder)
        {
            IdentityModelEventSource.ShowPII = true;

            builder.Services
                .AddAuthentication(option =>
                {
                    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = "http://localhost:8080/realms/Gem-E";
                    options.SaveToken = false;
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "http://localhost:8080/realms/Gem-E"
                    };
                });
        }
    }
}
