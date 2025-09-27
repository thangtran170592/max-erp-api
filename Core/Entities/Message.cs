using Core.Enums;

namespace Core.Entities
{
    public class Message : BaseEntity
    {
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public MessageStatus Status { get; set; }
        public virtual Conversation Conversation { get; set; } = null!;
        public virtual User Sender { get; set; } = null!;
        public virtual ICollection<MessageReceipt> Receipts { get; set; } = [];
    }
}