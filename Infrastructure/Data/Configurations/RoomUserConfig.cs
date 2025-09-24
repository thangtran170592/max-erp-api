using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class RoomUserConfig : IEntityTypeConfiguration<RoomUser>
    {
        public void Configure(EntityTypeBuilder<RoomUser> builder)
        {
            builder.ToTable("Room_Users");
            builder.HasKey(r => new { r.RoomId, r.UserId });

            builder
                .HasOne(r => r.Room)
                .WithMany(r => r.RoomUsers)
                .HasForeignKey(r => r.RoomId);

            builder
                .HasOne(r => r.User)
                .WithMany(u => u.Rooms)
                .HasForeignKey(r => r.UserId);
        }
    }
}