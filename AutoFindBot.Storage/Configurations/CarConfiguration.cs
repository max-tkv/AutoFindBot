using AutoFindBot.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFindBot.Storage.Configurations;

public class CarConfiguration : IEntityTypeConfiguration<Entities.Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.ToTable("Cars")
            .HasKey(x => x.Id);
        builder.HasIndex(x => new { x.Title, x.Price, x.Year, x.Source });
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("current_timestamp")
            .ValueGeneratedOnAdd()
            .IsRequired();
        builder.Property(x => x.UpdatedDateTime)
            .HasDefaultValueSql("current_timestamp")
            .ValueGeneratedOnAdd()
            .IsRequired();
        
        builder.Property(x => x.Vin)
            .IsRequired(false);
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.Cars)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}