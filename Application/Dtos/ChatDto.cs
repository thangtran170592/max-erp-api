using Core.Enums;

namespace Application.Dtos
{
    public record class MessageRequestDto : BaseDto
    {
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
    }

    public record class MessageResponseDto : BaseDto
    {
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public MessageStatus Status { get; set; }
        public ICollection<MessageReceiptDto> Receipts { get; set; } = [];
    }

    public record class ConversationRequestDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public ConversationType Type { get; set; }
        public Guid CreatorId { get; set; }
        public List<Guid> MemberIds { get; set; } = [];
    }

    public record class ConversationResponseDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public ConversationType Type { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public ICollection<ConversationMemberDto> Members { get; set; } = [];
        public bool IsVirtual { get; set; }
    }

    public record class BroadcastResponseDto : BaseDto
    {
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public bool IsScheduled { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public BroadcastStatus Status { get; set; }
        public ICollection<BroadcastRecipientDto> Recipients { get; set; } = [];
    }

    public record class MessageReceiptDto : BaseDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public ReceiptStatus Status { get; set; }
        public DateTime? ReadAt { get; set; }
    }

    public record class ConversationMemberDto : BaseDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime? LastReadAt { get; set; }
    }

    public record class BroadcastRecipientDto : BaseDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public ReceiptStatus Status { get; set; }
        public DateTime? ReadAt { get; set; }
    }

    public record class UpdateMessageStatusRequestDto : BaseDto
    {
        public ReceiptStatus Status { get; set; }
    }

    public record class BroadcastRequestDto : BaseDto
    {
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; } = MessageType.Text;
        public List<Guid> UserIds { get; set; } = [];
        public DateTime? ScheduledAt { get; set; }
    }

    public record class UserTypingRequestDto : BaseDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid ConversationId { get; set; }
    }
}
