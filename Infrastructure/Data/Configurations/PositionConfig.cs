using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PositionConfig : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.ToTable("Positions");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Uid)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(x => x.Uid)
                .IsUnique();

            builder.Property(x => x.Name).HasMaxLength(255);

            builder.Property(x => x.Status).HasDefaultValue(true);
        }
    }
}