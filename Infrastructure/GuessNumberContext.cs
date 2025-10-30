using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class GuessNumberContext : DbContext
    {
        public DbSet<ActualityEntity> Actualities { get; set; }

        public DbSet<CommunicationEntity> Communications { get; set; }


        public GuessNumberContext()
        {

        }

        public GuessNumberContext(DbContextOptions<GuessNumberContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                            v => v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                        ));
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>(
                            v => v.HasValue ? v.Value.ToUniversalTime() : v,
                            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
                        ));
                    }
                }
            }
        }
    }
}
