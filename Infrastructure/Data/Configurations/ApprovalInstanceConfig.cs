using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ApprovalInstanceConfig : IEntityTypeConfiguration<ApprovalInstance>
    {
        public void Configure(EntityTypeBuilder<ApprovalInstance> builder)
        {
            builder.ToTable("ApprovalInstances");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.Property(x => x.CurrentStepOrder)
                .IsRequired();

            builder.HasIndex(x => new { x.ApprovalFeatureId, x.DataId })
                .IsUnique();

            builder.Property(x => x.ReasonRejection)
                .HasMaxLength(1000);

            builder.HasMany(x => x.Actions)
                .WithOne(x => x.ApprovalInstance)
                .HasForeignKey(x => x.ApprovalInstanceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}