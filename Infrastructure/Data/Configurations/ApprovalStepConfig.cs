using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ApprovalStepConfig : IEntityTypeConfiguration<ApprovalStep>
    {
        public void Configure(EntityTypeBuilder<ApprovalStep> builder)
        {
            builder.ToTable("ApprovalSteps");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.StepOrder)
                .IsRequired();

            builder.HasIndex(x => new { x.ApprovalFeatureId, x.StepOrder })
                .IsUnique();

            builder.Property(x => x.TargetType)
                .HasConversion<int>();

            builder.Property(x => x.TargetId)
                .HasMaxLength(100);
        }
    }
}