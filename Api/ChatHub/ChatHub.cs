using System.IdentityModel.Tokens.Jwt;
using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Core.Constants;
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
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var senderSid = Context.User?.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
            var senderName = Context.User?.Identity?.Name ?? "Unknown";
            if (!string.IsNullOrEmpty(senderSid))
            {
                var connections = ConnectionMapping.GetConnections(senderSid!);
                if (connections is not null && connections.Any())
                {
                    _logger.LogWarning($"User {senderSid} already has {connections.Count()} connections");
                }
                ConnectionMapping.Add(senderSid, Context.ConnectionId);
            }
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
        var senderSid = Context.User?.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
        var senderName = Context.User?.Identity?.Name ?? "Unknown";
        _logger.LogInformation($"User  id: {senderSid}, name: {senderName} disconnected {Context.ConnectionId}");
        if (!string.IsNullOrEmpty(senderSid))
        {
            ConnectionMapping.Remove(senderSid, Context.ConnectionId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinUser(Guid? receiverId, Guid? roomId)
    {
        try
        {
            var senderSid = Context.User?.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
            var senderName = Context.User?.Identity?.Name ?? "Unknown";

            var groupId = string.Empty;
            if (roomId.HasValue)
            {
                groupId = $"Group-{roomId}";
            }
            else if (receiverId.HasValue)
            {
                groupId = GetPrivateGroupName(Guid.Parse(senderSid!), receiverId!.Value);
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            _logger.LogInformation($"User  id: {senderSid}, name: {senderName} joined group {groupId} successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding user to group");
            throw;
        }
    }

    public async Task SendMessage(MessageRequestDto request)
    {
        try
        {
            var senderSid = Context.User?.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
            var senderName = Context.User?.Identity?.Name ?? "Unknown";

            _logger.LogInformation($"Sending message: Id={senderSid}, name={senderName}, ReceiverId={request.ReceiverId}, RoomId={request.RoomId}, Type={request.Type}");

            if (string.IsNullOrEmpty(senderSid))
                throw new HubException("User not authenticated");

            var senderId = Guid.Parse(senderSid);
            request.SenderId = senderId;
            request.SenderName = senderName;
            var sentMessage = await _chatService.SendMessage(request);

            if (request.RoomId.HasValue)
            {
                var roomUsers = await _chatService.GetMessages(request.RoomId, null, null);
                foreach (var user in roomUsers)
                    await Clients.Group($"Group-{request.RoomId}").SendAsync(ChatHubEvent.ReceiveMessage, sentMessage);
            }
            else if (request.ReceiverId.HasValue)
            {
                var groupName = GetPrivateGroupName(senderId, request.ReceiverId.Value);
                await Clients.Group(groupName).SendAsync(ChatHubEvent.ReceiveMessage, sentMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error send message");
            throw;
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
        {
            await Clients.Group($"User-{uid}").SendAsync("ReceiveMessage", senderId, content, type, DateTime.UtcNow);
        }
    }

    private string GetPrivateGroupName(Guid sender1, Guid sender2)
    {
        var sorted = new[] { sender1, sender2 }.OrderBy(x => x).ToArray();
        return $"Private-{sorted[0]}-{sorted[1]}";
    }
}
