using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ApprovalRequestConfig : IEntityTypeConfiguration<ApprovalRequest>
    {
        public void Configure(EntityTypeBuilder<ApprovalRequest> builder)
        {
            builder.ToTable("ApprovalRequests");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.Property(x => x.CurrentStepOrder)
                .IsRequired();

            builder.HasIndex(x => new { x.ApprovalFeatureId, x.DataId })
                .IsUnique();

            builder.Property(x => x.ReasonRejection)
                .HasMaxLength(1000);

            builder.HasMany(x => x.ApprovalHistories)
                .WithOne(x => x.ApprovalRequest)
                .HasForeignKey(x => x.ApprovalRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}