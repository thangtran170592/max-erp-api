using Core.Enums;

namespace Core.Entities
{
    public class Message : BaseEntity
    {
        public Guid SenderId { get; set; }
        public Guid? ReceiverId { get; set; }
        public Guid? RoomId { get; set; }
        public string Content { get; set; } = null!;
        public MessageType Type { get; set; } = MessageType.Text;
        public bool IsRead { get; set; } = false;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public virtual User Sender { get; set; } = null!;
        public virtual User? Receiver { get; set; }
        public virtual Room? Room { get; set; }
    }
}