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
            await Clients.All.SendAsync(ChatHubEvent.UserOnline, senderSid);
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
        await Clients.All.SendAsync(ChatHubEvent.UserOffline, senderSid);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinUser(Guid? receiverId, Guid? roomId, string? roomName)
    {
        try
        {
            var senderSid = Context.User?.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
            var senderName = Context.User?.Identity?.Name ?? "Unknown";

            var groupId = string.Empty;
            if (roomId.HasValue)
            {
                groupId = $"Group-{roomId}";
                if (!string.IsNullOrEmpty(roomName))
                {
                    groupId += $"-{StringHelper.ToFriendlyUrl(roomName)}";
                }
            }
            else if (receiverId.HasValue)
            {
                groupId = ChatHubHelper.GetGroupName(Guid.Parse(senderSid!), receiverId!.Value);
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

    public async Task JoinConversation(string conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        await Clients.Group(conversationId).SendAsync("UserJoined", Context.UserIdentifier, conversationId);
    }

    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        await Clients.Group(conversationId).SendAsync(ChatHubEvent.UserLeft, Context.UserIdentifier, conversationId);
    }

    public async Task Typing(string conversationId)
    {
        await Clients.OthersInGroup(conversationId)
             .SendAsync(ChatHubEvent.UserTyping, Context.UserIdentifier, conversationId);
    }

    public async Task SeenMessage(string conversationId, string messageId)
    {
        await Clients.Group(conversationId).SendAsync(ChatHubEvent.SeenMessage, Context.UserIdentifier, messageId, DateTime.UtcNow);
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
                var groupId = $"Group-{request.RoomId}";
                if (!string.IsNullOrEmpty(request.RoomName))
                {
                    groupId += $"-{StringHelper.ToFriendlyUrl(request.RoomName)}";
                }
                await Clients.Group(groupId).SendAsync(ChatHubEvent.ReceiveMessage, sentMessage);
            }
            else if (request.ReceiverId.HasValue)
            {
                var groupName = ChatHubHelper.GetGroupName(senderId, request.ReceiverId.Value);
                await Clients.Group(groupName).SendAsync(ChatHubEvent.ReceiveMessage, sentMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error send message");
            throw;
        }
    }

    public async Task SendBroadcast(BroadcastRequestDto request)
    {
        var senderSid = Context.User?.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
        var senderName = Context.User?.Identity?.Name ?? "Unknown";
        request.SenderId = Guid.Parse(senderSid!);
        request.SenderName = senderName;
        var userIds = await _chatService.SendBroadcast(request);
        if (userIds is not null && userIds.Any())
        {
            var message = new MessageResponseDto
            {
                SenderId = request.SenderId,
                Content = request.Content,
                SenderName = senderName,
                Type = request.Type
            };
            foreach (var uid in userIds)
            {
                message.ReceiverId = uid;
                await Clients.Group($"Private-{senderSid}-{uid}").SendAsync(ChatHubEvent.ReceiveMessage, message);
            }
        }
    }
}
