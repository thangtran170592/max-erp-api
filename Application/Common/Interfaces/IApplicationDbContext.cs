using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Room> Rooms { get; }
        DbSet<RoomUser> RoomUsers { get; }
        DbSet<Message> Messages { get; }
        DbSet<Broadcast> Broadcasts { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}