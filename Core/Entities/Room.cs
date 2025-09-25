namespace Core.Entities
{
    public class Room : BaseEntity
    {
        public string RoomCode { get; set; } = null!;
        public bool IsGroup { get; set; } = false;
        public string Name { get; set; } = null!;
        public Guid CreatedBy { get; set; }
        public User Creator { get; set; } = null!;

        public virtual ICollection<RoomUser> RoomUsers { get; set; } = new List<RoomUser>();
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}