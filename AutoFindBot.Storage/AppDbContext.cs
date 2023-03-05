using Microsoft.EntityFrameworkCore;
using AutoFindBot.Storage.Configurations;

namespace AutoFindBot.Storage
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            base.ChangeTracker.AutoDetectChangesEnabled = true;
            base.Database.AutoTransactionsEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder
                .ApplyConfiguration(new ActionConfiguration())
                .ApplyConfiguration(new AppUserConfiguration())
                .ApplyConfiguration(new PaymentConfiguration())
                .ApplyConfiguration(new CarConfiguration())
                .ApplyConfiguration(new UserFilterConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }
}