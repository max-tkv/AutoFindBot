using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFindBot.Storage.Configurations;

public class ActionConfiguration : IEntityTypeConfiguration<Entities.Action>
{
    public void Configure(EntityTypeBuilder<Entities.Action> builder)
    {
        builder.ToTable("Actions")
            .HasKey(x => x.Id);
        builder.HasIndex(x => x.CommandName);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("current_timestamp")
            .ValueGeneratedOnAdd()
            .IsRequired();
        builder.Property(x => x.UpdatedDateTime)
            .HasDefaultValueSql("current_timestamp")
            .ValueGeneratedOnAdd()
            .IsRequired();
        
        builder.Property(x => x.ActionText)
            .IsRequired(false);
        builder.Property(x => x.Category)
            .IsRequired(false);
        builder.Property(x => x.PageData)
            .IsRequired(false);
        builder.Property(x => x.Page)
            .HasDefaultValue(0);
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.Actions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}