using Application.Dtos;
using Core.Enums;

namespace Application.IServices
{
    public interface IChatService
    {
        Task<MessageResponseDto> SendMessage(MessageRequestDto request);
        Task<ConversationResponseDto> CreateConversation(ConversationRequestDto request);
        Task<BroadcastResponseDto> CreateBroadcastAsync(Guid senderId, string content, MessageType type, List<Guid> userIds, DateTime? scheduledAt = null);
        Task<IEnumerable<MessageResponseDto>> GetConversationMessages(Guid conversationId, int skip = 0, int take = 50);
        Task UpdateMessageStatus(Guid messageId, Guid userId, ReceiptStatus status);
        Task<IEnumerable<ConversationResponseDto>> GetUserConversations(Guid userId);
    }
}