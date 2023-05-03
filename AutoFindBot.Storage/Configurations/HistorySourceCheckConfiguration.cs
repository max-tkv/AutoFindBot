using AutoFindBot.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFindBot.Storage.Configurations;

public class SourceCheckConfiguration : IEntityTypeConfiguration<Entities.SourceCheck>
{
    public void Configure(EntityTypeBuilder<SourceCheck> builder)
    {
        builder.ToTable("SourceChecks")
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
            .WithMany(x => x.SourceChecks)
            .HasForeignKey(x => x.UserFilterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}