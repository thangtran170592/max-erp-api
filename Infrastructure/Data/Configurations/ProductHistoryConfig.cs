using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ProductHistoryConfig : IEntityTypeConfiguration<ProductHistory>
    {
        public void Configure(EntityTypeBuilder<ProductHistory> builder)
        {
            builder.ToTable("ProductHistories");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.Property(x => x.Uid)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.Length)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Width)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Height)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Type)
                .IsRequired();

            builder.Property(x => x.ApprovalStatus)
                .IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}