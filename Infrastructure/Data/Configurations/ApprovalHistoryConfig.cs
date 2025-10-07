using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ApprovalHistoryConfig : IEntityTypeConfiguration<ApprovalHistory>
    {
        public void Configure(EntityTypeBuilder<ApprovalHistory> builder)
        {
            builder.ToTable("ApprovalHistories");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.Property(x => x.StepOrder)
                .IsRequired();

            builder.Property(x => x.Reason)
                .HasMaxLength(1000);

            builder.HasIndex(x => new { x.ApprovalRequestId, x.StepOrder });
        }
    }
}