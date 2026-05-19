using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Logging;
using UserAPI.Models;

namespace UserAPI.Extensions
{
    public static class AuthExtensions
    {
        private static void ConfigureRolesAndPolicies(this AuthorizationOptions options)
        {
            options.AddPolicy(Policies.BANNED_USER_POLICY, policy => policy.RequireRole(Roles.BANNED_USER));
            options.AddPolicy(Policies.USER_POLICY, policy => policy.RequireRole(Roles.USER));
            options.AddPolicy(Policies.ANY_USER_POLICY, policy => policy.RequireRole(Roles.USER, Roles.BANNED_USER));
        }

        public static void ConfigureSwaggerGen(this SwaggerGenOptions options)
        {
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "bearer"
            });
            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        }

        public static void AddAuthBuilderExtension(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new Exception()))
                    };
                    options.SaveToken = true;
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var loggerFactory = context.HttpContext.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
                            var logger = loggerFactory?.CreateLogger("JwtBearer");
                            logger?.LogError(context.Exception, "JWT authentication failed.");
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            var loggerFactory = context.HttpContext.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
                            var logger = loggerFactory?.CreateLogger("JwtBearer");
                            logger?.LogWarning("JWT challenge. Error: {Error}, Description: {Description}", context.Error, context.ErrorDescription);
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddAuthorization(options => options.ConfigureRolesAndPolicies());
        }

        public static void AddAuthAppExtension(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
