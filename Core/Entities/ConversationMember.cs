using Core.Constants;

namespace Core.Entities
{
    public class ConversationMember : BaseEntity
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; } = ChatRole.Member;
        public DateTime? LastReadAt { get; set; }
        public virtual Conversation Conversation { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}