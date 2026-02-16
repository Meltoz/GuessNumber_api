using Application.Interfaces.Repository;
using Application.Services;
using Infrastructure.Mappings;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config, string env)
        {

            // 1️ Si on n'est pas en Testing, on configure la base PostgreSQL
            if (env != "Testing")
            {
                var defaultConnection = config.GetConnectionString("DefaultConnection");

                if (string.IsNullOrWhiteSpace(defaultConnection))
                    throw new Exception("No configuration for database");

                services.AddDbContext<GuessNumberContext>((serviceProvider, options) =>
                {
                    options.UseNpgsql(defaultConnection);
                });
            }

            services.AddScoped<IActualityRepository, ActualityRepository>();
            services.AddScoped<ICommunicationRepository, CommunicationRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IProposalRepository, ProposalRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthUserRepository, AuthUserRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();

            services.AddAutoMapper(cfg => { cfg.ShouldUseConstructor = ci => ci.IsPrivate; }, typeof(DomainToEntityProfile), typeof(EntityToDomainProfile));
            return services;
        }

    }
}
