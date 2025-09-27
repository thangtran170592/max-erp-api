using Core.Enums;

namespace Application.Dtos
{
    public class MessageRequestDto
    {
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
    }

    public class MessageResponseDto
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public MessageStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<MessageReceiptDto> Receipts { get; set; } = [];
    }

    public class ConversationRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public ConversationType Type { get; set; }
        public Guid CreatorId { get; set; }
        public List<Guid> MemberIds { get; set; } = [];
    }

    public class ConversationResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ConversationType Type { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public ICollection<ConversationMemberDto> Members { get; set; } = [];
        public bool IsVirtual { get; set; }
    }

    public class BroadcastResponseDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public bool IsScheduled { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public BroadcastStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<BroadcastRecipientDto> Recipients { get; set; } = [];
    }

    public class MessageReceiptDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public ReceiptStatus Status { get; set; }
        public DateTime? ReadAt { get; set; }
    }

    public class ConversationMemberDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime? LastReadAt { get; set; }
    }
    
    public class BroadcastRecipientDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public ReceiptStatus Status { get; set; }
        public DateTime? ReadAt { get; set; }
    }

    public class UpdateMessageStatusRequest
    {
        public ReceiptStatus Status { get; set; }
    }

    public class BroadcastRequestDto
    {
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; } = MessageType.Text;
        public List<Guid> UserIds { get; set; } = [];
        public DateTime? ScheduledAt { get; set; }
    }
}
