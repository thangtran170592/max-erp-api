using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PackageUnitConfig : IEntityTypeConfiguration<PackageUnit>
    {
        public void Configure(EntityTypeBuilder<PackageUnit> builder)
        {
            builder.ToTable("PackageUnits");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Uid)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Level)
                .IsRequired();

            builder.Property(x => x.Quantity)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.UnitId)
                .IsRequired();

            builder.HasOne(x => x.Unit)
                .WithMany()
                .HasForeignKey(x => x.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}