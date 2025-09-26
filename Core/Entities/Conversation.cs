using Core.Enums;

namespace Core.Entities
{
    public class Conversation : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public ConversationType Type { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public virtual ICollection<ConversationMember> Members { get; set; } = [];
        public virtual ICollection<Message> Messages { get; set; } = [];
    }
}