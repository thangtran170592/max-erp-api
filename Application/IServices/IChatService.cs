using Application.Dtos;
using Core.Enums;

namespace Application.IServices
{
    public interface IChatService
    {
        Task<IEnumerable<UserResponseDto>> GetChatAbleUsers(Guid userId);
        Task<IEnumerable<MessageResponseDto>> GetMessages(Guid? roomId, Guid? userId1, Guid? userId2);
        Task<MessageResponseDto?> SendMessage(MessageRequestDto request);
        Task<RoomResponseDto?> ValidateRoom(string roomCode, bool isGroup);
        Task<RoomResponseDto> CreateRoom(RoomRequestDto request);
        Task<IEnumerable<RoomResponseDto>> GetUserRooms(Guid userId);
        Task<List<Guid>> SendBroadcast(BroadcastRequestDto request);
    }
}