using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure
{
    public class GuessNumberContext : DbContext
    {
        private readonly bool _disableTimestamps;

        public DbSet<ActualityEntity> Actualities { get; set; }

        public DbSet<CommunicationEntity> Communications { get; set; }

        public DbSet<ReportEntity> Reports { get; set; }

        public DbSet<CategoryEntity> Categories { get; set; }

        public DbSet<QuestionEntity> Questions { get; set; }

        public DbSet<ProposalEntity> Proposals { get; set; }


        public GuessNumberContext()
        {

        }

        public GuessNumberContext(DbContextOptions<GuessNumberContext> options, bool disableTimestamps = false)
       : base(options)
        {
            _disableTimestamps = disableTimestamps;
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

            modelBuilder.Entity<CategoryEntity>(entity =>
            {
                entity.HasMany(c => c.Questions)
                .WithOne(c => c.Category)
                .HasForeignKey(q => q.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<QuestionEntity>(entity =>
            {
                entity.Property(q => q.Visibility)
                .HasConversion<int>();

                entity.Property(q => q.Type)
                .HasConversion<int>();
            });

        }

        public override int SaveChanges()
        {
            if (!_disableTimestamps)
            {
                UpdateTimestamps();
            }
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (!_disableTimestamps)
            {
                UpdateTimestamps();
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            var now = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = now;
                        entry.Entity.Updated = now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.Updated = now;
                        // Empêche la modification de Created
                        entry.Property(nameof(BaseEntity.Created)).IsModified = false;
                        break;
                }
            }
        }
    }
}
