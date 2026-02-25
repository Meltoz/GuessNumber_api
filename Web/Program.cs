using Application;
using Application.Interfaces.Repository;
using Application.Interfaces.Web;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Web.Constants;
using Web.Hubs;
using Web.Mappings;
using Web.Middlewares;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var env = builder.Environment.EnvironmentName;
            services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection("ConnectionStrings"));
            services.Configure<AuthConfiguration>(builder.Configuration.GetSection("Auth"));
            services.Configure<EncryptionConfiguration>(builder.Configuration.GetSection("Encryption"));

            var authConfiguration = builder.Configuration
                .GetSection("Auth")
                .Get<AuthConfiguration>() ?? throw new Exception("No configuration for auth");

            if (env != "Testing")
            {
                var connections = builder.Configuration
                    .GetSection("ConnectionStrings")
                    .Get<DatabaseConfiguration>() ?? throw new Exception("No configuration for database");
            }
            var allowedOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>();

            if (allowedOrigins is null)
            {
                throw new InvalidOperationException("CORS: AllowedOrigins est vide ou manquant");
            }

            services.AddCors(opt =>
            {
                opt.AddPolicy("front", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders(ApiConstants.TotalCountHeader);
                });
            });

            // Add services to the container.

            services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddOpenApi();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Configure DI Service
            services.AddApplication();
            services.AddInfrastructure(builder.Configuration, env);
            services.AddAutoMapper(cfg => { }, typeof(ViewModelToDomainProfile), typeof(DomainToViewModelProfile));
            services.AddSignalR();
            services.AddScoped<IGameHubNotifier, GameHubNotifier>();


            // Authentification
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfiguration.Key)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true
                    };

                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            ctx.Token = ctx.Request.Cookies[ApiConstants.AccessTokenCookieName];
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = async ctx =>
                        {
                            var jtiClaim = ctx.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                            if(!Guid.TryParse(jtiClaim, out var tokenId))
                            {
                                ctx.Fail("Invalid Token");
                                return;
                            }
                            var tokenRepo = ctx.HttpContext.RequestServices
                                .GetService<ITokenRepository>();
                            var token = await tokenRepo.GetByIdAsync(tokenId);

                            if (token is null || token.IsRevoked)
                                ctx.Fail("Token has been revoked");
                        }
                    };
                });

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy(ApiConstants.AdminPolicy, 
                    policy => policy.RequireRole("Admin"));

                opt.AddPolicy(ApiConstants.AuthenticatedUserPolicy, 
                    policy => policy.RequireAuthenticatedUser());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //Application des migrationsAhh
            if (!app.Environment.IsEnvironment("Testing"))
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<GuessNumberContext>();
                dbContext.Database.Migrate();
            }


            // Middlewares
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();
            app.UseCors("front");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHub<GameHub>("/hubs/game");

            app.Run();
        }
    }
}
