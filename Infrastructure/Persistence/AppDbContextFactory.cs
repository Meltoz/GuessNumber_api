using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<GuessNumberContext>
    {
        public GuessNumberContext CreateDbContext(string[] args)
        {
            // Résolution du chemin absolu vers le projet Web (ajuste si nécessaire)
            var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../Web"));

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath) 
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection")
                                   ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            var optionsBuilder = new DbContextOptionsBuilder<GuessNumberContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new GuessNumberContext(optionsBuilder.Options, null);
        }
    }
}
