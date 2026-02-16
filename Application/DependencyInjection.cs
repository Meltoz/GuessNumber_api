using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ActualityService>();
            services.AddScoped<CommunicationService>();
            services.AddScoped<ReportService>();
            services.AddScoped<CategoryService>();
            services.AddScoped<QuestionService>();
            services.AddScoped<ProposalService>();
            services.AddScoped<UserService>();
            services.AddScoped<AesEncryptionService>();
            services.AddScoped<JwtService>();
            services.AddScoped<TokenService>();


            return services;
        }
    }
}
