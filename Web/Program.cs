using Application;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shared.Configuration;
using Web.Constants;
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

            app.UseCors("front");

            // Middlewares
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
