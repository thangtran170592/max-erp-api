using Core.Enums;

namespace Core.Entities
{
    public class Broadcast : BaseEntity
    {
        public Guid SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public bool IsScheduled { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public BroadcastStatus Status { get; set; }
        
        public virtual User Sender { get; set; } = null!;
        public virtual ICollection<BroadcastRecipient> Recipients { get; set; } = [];
    }
}