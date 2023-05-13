using AutoFindBot.Entities;
using Microsoft.EntityFrameworkCore;
using AutoFindBot.Storage.Configurations;
using Action = AutoFindBot.Entities.Action;

namespace AutoFindBot.Storage
{
    public class AppDbContext : DbContext
    {
        public DbSet<AppUser> Users { get; set; }
        
        public DbSet<Action> Actions { get; set; }
        
        public DbSet<Car> Cars { get; set; }
        
        public DbSet<Payment> Payments { get; set; }
        
        public DbSet<SourceCheck> SourceChecks { get; set; }
        
        public DbSet<UserFilter> UserFilters { get; set; }
        
        public DbSet<Source> Sources { get; set; }
        
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
                .ApplyConfiguration(new UserFilterConfiguration())
                .ApplyConfiguration(new SourceCheckConfiguration())
                .ApplyConfiguration(new SourceConfiguration());
        }
    }
}