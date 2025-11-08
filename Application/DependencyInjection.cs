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


            return services;
        }
    }
}
