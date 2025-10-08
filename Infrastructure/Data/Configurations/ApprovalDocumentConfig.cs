using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ApprovalDocumentConfig : IEntityTypeConfiguration<ApprovalDocument>
    {
        public void Configure(EntityTypeBuilder<ApprovalDocument> builder)
        {
            builder.ToTable("ApprovalDocuments");
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
                .WithOne(x => x.ApprovalDocument)
                .HasForeignKey(x => x.ApprovalDocumentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}