using AutoFindBot.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFindBot.Storage.Configurations;

public class HistorySourceCheckConfiguration : IEntityTypeConfiguration<Entities.HistorySourceCheck>
{
    public void Configure(EntityTypeBuilder<HistorySourceCheck> builder)
    {
        builder.ToTable("HistorySourceChecks")
            .HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("current_timestamp")
            .ValueGeneratedOnAdd()
            .IsRequired();
        builder.Property(x => x.UpdatedDateTime)
            .HasDefaultValueSql("current_timestamp")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.HasOne(x => x.UserFilter)
            .WithMany(x => x.HistorySourceChecks)
            .HasForeignKey(x => x.UserFilterId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(x => x.Cars)
            .WithOne(x => x.HistorySourceCheck)
            .HasForeignKey(x => x.HistorySourceCheckId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}