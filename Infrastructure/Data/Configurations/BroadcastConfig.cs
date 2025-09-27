using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class BroadcastConfig : IEntityTypeConfiguration<Broadcast>
    {
        public void Configure(EntityTypeBuilder<Broadcast> builder)
        {
            builder.ToTable("Broadcasts");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.HasOne(x => x.Sender)
                .WithMany()
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Recipients)
                .WithOne(x => x.Broadcast)
                .HasForeignKey(x => x.BroadcastId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}