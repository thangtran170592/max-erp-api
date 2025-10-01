using Core.Enums;

namespace Core.Entities
{
    public class ConversationMember : BaseEntity
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; } = ChatRole.Member.GetTitle();
        public DateTime? LastReadAt { get; set; }
        public virtual Conversation Conversation { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}