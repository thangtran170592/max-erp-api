using Core.Entities;
using Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ApprovalConfig : IEntityTypeConfiguration<Core.Entities.ApprovalConfig>
    {
        public void Configure(EntityTypeBuilder<Core.Entities.ApprovalConfig> builder)
        {
            builder.ToTable("ApprovalConfigs");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Uid)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(x => x.Uid)
                .IsUnique();

            builder.Property(x => x.ApprovalGroup)
                .IsRequired();

            builder.Property(x => x.Name).HasMaxLength(255);
            builder.Property(x => x.Description).HasMaxLength(1000);

            builder.Property(x => x.Status).HasDefaultValue(true);

            builder.HasMany(x => x.ApprovalFeatures)
                .WithOne(x => x.ApprovalConfig)
                .HasForeignKey(x => x.ApprovalConfigId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}