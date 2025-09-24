using Application.Dtos;
using Core.Enums;

namespace Application.IServices
{
    public interface IChatService
    {
        Task<IEnumerable<UserResponseDto>> GetChatAbleUsers(Guid userId);
        Task<IEnumerable<MessageResponseDto>> GetMessages(Guid? roomId, Guid? userId1, Guid? userId2);
        Task<int> SendMessage(Guid senderId, Guid? receiverId, Guid? roomId, string content, MessageType type = MessageType.Text);
        Task<RoomResponseDto> CreateRoom(string name, Guid adminId, List<Guid> userIds);
        Task<IEnumerable<RoomResponseDto>> GetUserRooms(Guid userId);
        Task<int> SendBroadcast(Guid senderId, string content, MessageType type, string targetType, Guid? targetId);
    }
}