using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ApprovalFeatureConfig : IEntityTypeConfiguration<ApprovalFeature>
    {
        public void Configure(EntityTypeBuilder<ApprovalFeature> builder)
        {
            builder.ToTable("ApprovalFeatures");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Uid)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Status)
                .HasDefaultValue(true);

            builder.HasIndex(x => new { x.ApprovalConfigId, x.Id })
                .IsUnique();

            builder.HasMany(x => x.ApprovalSteps)
                .WithOne(x => x.ApprovalFeature)
                .HasForeignKey(x => x.ApprovalFeatureId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}