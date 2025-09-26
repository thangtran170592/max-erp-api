using Core.Enums;

namespace Core.Entities
{
    public class MessageReceipt : BaseEntity
    {
        public Guid MessageId { get; set; }
        public Guid UserId { get; set; }
        public ReceiptStatus Status { get; set; }
        public DateTime? ReadAt { get; set; }
        public virtual Message Message { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}