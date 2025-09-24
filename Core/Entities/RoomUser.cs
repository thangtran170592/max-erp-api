namespace Core.Entities
{
    public class RoomUser
    {
        public Guid RoomId { get; set; }
        public virtual Room Room { get; set; } = null!;
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}