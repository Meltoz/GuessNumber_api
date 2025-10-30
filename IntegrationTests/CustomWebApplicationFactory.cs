using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web;

namespace Meltix.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly GuessNumberContext _externalContext;

        public CustomWebApplicationFactory(GuessNumberContext context)
        {
            _externalContext = context;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // 🧹 Supprimer le contexte existant du pipeline DI
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<GuessNumberContext>)
                );
                if (descriptor != null)
                    services.Remove(descriptor);

                // 🧩 Injecter ton propre contexte externe (fourni depuis le test)
                services.AddSingleton(_externalContext);

                // ⚠️ Si tu veux t’assurer que EF ne tente pas de le disposer
                services.AddDbContext<GuessNumberContext>(options =>
                {
                    options.UseSqlite(_externalContext.Database.GetDbConnection());
                });
            });
        }
    }
}

