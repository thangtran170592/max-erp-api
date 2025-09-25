using Core.Entities;
using Core.Enums;

namespace Application.Dtos
{
    public record class MessageRequestDto
    {
        public Guid? RoomId { get; init; }
        public string? RoomName { get; init; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public Guid? ReceiverId { get; init; }
        public string Content { get; init; } = null!;
        public MessageType Type { get; init; } = MessageType.Text;
        public bool IsRead { get; init; } = false;
    }

    public record class MessageResponseDto
    {
        public Guid Id { get; init; }
        public Guid SenderId { get; init; }
        public string SenderName { get; set; } = string.Empty;
        public Guid? ReceiverId { get; set; }
        public Guid? RoomId { get; init; }
        public string Content { get; init; } = null!;
        public MessageType Type { get; init; } = MessageType.Text;
        public bool IsRead { get; init; } = false;
    }

    public record class RoomRequestDto
    {
        public string HostId { get; init; } = string.Empty;
        public string RoomCode { get; init; } = string.Empty;
        public bool IsGroup { get; init; } = false;
        public string Name { get; init; } = null!;
        public List<Guid> UserIds { get; init; } = [];
        public Guid CreatedBy { get; init; }
    }

    public record class RoomResponseDto
    {
        public Guid Id { get; init; }
        public string RoomCode { get; init; } = string.Empty;
        public string Name { get; init; } = null!;
        public Guid CreatedBy { get; init; }
    }

    public record class HistoryMessagesRequestDto
    {
        public Guid? RoomId { get; init; }
        public Guid? CustomerId { get; init; }
    }

    public record class BroadcastRequestDto
    {
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; init; } = string.Empty;
        public MessageType Type { get; init; }
        public TargetType TargetType { get; init; }
        public Guid TargetId { get; init; }
    }
}