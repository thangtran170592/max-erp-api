using Core.Enums;

namespace Core.Entities
{
    public class BroadcastRecipient : BaseEntity
    {
        public Guid BroadcastId { get; set; }
        public Guid UserId { get; set; }
        public ReceiptStatus Status { get; set; }
        public DateTime? ReadAt { get; set; }
        
        public virtual Broadcast Broadcast { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}