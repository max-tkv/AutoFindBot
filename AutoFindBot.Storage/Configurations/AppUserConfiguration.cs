using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoFindBot.Entities;

namespace AutoFindBot.Storage.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<Entities.AppUser>
{
    public void Configure(EntityTypeBuilder<Entities.AppUser> builder)
    {
        builder.ToTable("Users")
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
        
        builder.Property(x => x.Tarif)
            .HasDefaultValue(Tarif.Free);

        builder.HasMany(x => x.Actions)
            .WithOne(x => x.User)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Payments)
            .WithOne(x => x.User)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Cars)
            .WithOne(x => x.User)
            .OnDelete(DeleteBehavior.Restrict);
    }
}