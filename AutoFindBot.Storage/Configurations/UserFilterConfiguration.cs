using AutoFindBot.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFindBot.Storage.Configurations;

public class UserFilterConfiguration : IEntityTypeConfiguration<Entities.UserFilter>
{
    public void Configure(EntityTypeBuilder<UserFilter> builder)
    {
        builder.ToTable("UserFilters")
            .HasKey(x => x.Id);
        builder.Property(x => x.PriceMin)
            .HasColumnType("decimal(10,2)");
        builder.Property(x => x.PriceMax)
            .HasColumnType("decimal(10,2)");
        builder.HasIndex(x => new { x.Title });
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("current_timestamp")
            .ValueGeneratedOnAdd()
            .IsRequired();
        builder.Property(x => x.UpdatedDateTime)
            .HasDefaultValueSql("current_timestamp")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserFilters)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(x => x.Cars)
            .WithOne(x => x.UserFilter)
            .HasForeignKey(x => x.UserFilterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}