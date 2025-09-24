using Application.IServices;
using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api.ChatHub;

[Authorize]
public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;
    private readonly IChatService _chatService;
    public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _logger = logger;
        _logger.LogDebug("ChatHub constructed");
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            _logger.LogDebug("Attempting to connect client");
            _logger.LogInformation($"Client Connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
            _logger.LogDebug("Client successfully connected");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnConnectedAsync");
            throw;
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation($"Client Disconnected: {Context.ConnectionId}");
        Console.WriteLine($"Client Disconnected: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinUser(int userId)
    {
        try
        {
            _logger.LogDebug($"Attempting to add user {userId} to group");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User-{userId}");
            _logger.LogInformation($"User {userId} joined group successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding user {userId} to group");
            throw;
        }
    }

    public async Task SendMessage(Guid senderId, Guid? receiverId, Guid? roomId, string content, MessageType type)
    {
        _logger.LogInformation($"Sending message: SenderId={senderId}, ReceiverId={receiverId}, RoomId={roomId}, Type={type}");

        await _chatService.SendMessage(senderId, receiverId, roomId, content, type);

        if (roomId.HasValue)
        {
            var roomUsers = await _chatService.GetMessages(roomId, null, null);
            foreach (var user in roomUsers)
                await Clients.Group($"User-{user.ReceiverId}").SendAsync("ReceiveMessage", senderId, content, type, DateTime.UtcNow);
        }
        else if (receiverId.HasValue)
        {
            await Clients.Group($"User-{receiverId}").SendAsync("ReceiveMessage", senderId, content, type, DateTime.UtcNow);
            await Clients.Group($"User-{senderId}").SendAsync("ReceiveMessage", senderId, content, type, DateTime.UtcNow);
        }
    }

    public async Task SendBroadcast(Guid senderId, string content, MessageType type, string targetType, Guid? targetId)
    {
        await _chatService.SendBroadcast(senderId, content, type, targetType, targetId);

        List<Guid> userIds = targetType switch
        {
            "All" => await _chatService.GetChatAbleUsers(senderId).ContinueWith(t => t.Result.Select(u => u.Id).ToList()),
            "Group" => await _chatService.GetMessages(targetId, null, null).ContinueWith(t => t.Result.Select(m => m.ReceiverId ?? Guid.NewGuid()).ToList()),
            _ => []
        };

        foreach (var uid in userIds)
            await Clients.Group($"User-{uid}").SendAsync("ReceiveMessage", senderId, content, type, DateTime.UtcNow);
    }
}
