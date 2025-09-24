using Core.Entities;
using Core.Enums;

namespace Application.Dtos
{
    public record class MessageResponseDto
    {
        public Guid Id { get; init; }
        public Guid SenderId { get; init; }
        public Guid? ReceiverId { get; init; }
        public Guid? RoomId { get; init; }
        public string Content { get; init; } = null!;
        public MessageType Type { get; init; } = MessageType.Text;
        public bool IsRead { get; init; } = false;
        public DateTime SentAt { get; init; } = DateTime.UtcNow;
    }

    public record class RoomResponseDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public Guid CreatedBy { get; init; }
        public User Creator { get; init; } = null!;
    }

    public record class SendMessageRequestDto
    {
        public Guid ReceiverId { get; init; }
        public Guid RoomId { get; init; }
        public string Content { get; init; } = string.Empty;
        public MessageType Type { get; init; }
    }

    public record class CreateRoomRequestDto
    {
        public string Name { get; init; } = string.Empty;
        public List<Guid> UserIds { get; init; } = [];
    }

    public record class BroadcastRequestDto
    {
        public string Content { get; init; } = string.Empty;
        public MessageType Type { get; init; }
        public string TargetType { get; init; } = string.Empty;
        public Guid TargetId { get; init; }
    }
}