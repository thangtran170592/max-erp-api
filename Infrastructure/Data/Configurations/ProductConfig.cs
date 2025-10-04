using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Uid)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

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

            builder.Property(x => x.ApprovalStatus)
                .IsRequired();
                
            builder.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Package)
                .WithMany()
                .HasForeignKey(x => x.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UnitOfMeasure)
                .WithMany()
                .HasForeignKey(x => x.UnitOfMeasureId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}