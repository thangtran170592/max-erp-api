using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class BarcodeConfig : IEntityTypeConfiguration<Barcode>
    {
        public void Configure(EntityTypeBuilder<Barcode> b)
        {
            b.ToTable("Barcodes");
            b.HasKey(x => x.Id);

            b.Property(x => x.BarcodeForType).HasConversion<int>();
            b.Property(x => x.BarcodeType).HasConversion<int>();

            b.Property(x => x.Content)
                .IsRequired()
                .HasMaxLength(256);
            b.Property(x => x.Data)
                .IsRequired()
                .HasMaxLength(64);

            b.Property(x => x.Size).HasConversion<int>().IsRequired();

            b.HasIndex(x => new { x.BarcodeForType, x.UserId })
                .IsUnique();
        }
    }
}