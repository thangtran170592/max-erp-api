using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class WarehouseHistoryConfig : IEntityTypeConfiguration<WarehouseHistory>
    {
        public void Configure(EntityTypeBuilder<WarehouseHistory> builder)
        {
            builder.ToTable("WarehouseHistories");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.WarehouseId)
                .IsRequired();

            builder.Property(x => x.Uid)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.ApprovalStatus)
                .IsRequired();

            builder.Property(x => x.ChangedAt)
                .IsRequired();

            builder.Property(x => x.ChangedBy)
                .HasMaxLength(128);

            builder.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}