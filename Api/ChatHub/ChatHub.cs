using System.IdentityModel.Tokens.Jwt;
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
            var userId = GetCurrentUserId();
            var userName = Context.User?.Identity?.Name ?? "Unknown";

            var connections = ConnectionMapping.GetConnections(userId.ToString());
            if (connections?.Any() == true)
            {
                _logger.LogInformation($"User {userId} has {connections.Count()} existing connections");
            }

            ConnectionMapping.Add(userId.ToString(), Context.ConnectionId);
            await UpdateUserStatus(userId, UserStatus.Available);
            await Clients.Others.SendAsync(ChatHubEvent.UserOnline, userId);
            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnConnectedAsync");
            throw;
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userName = Context.User?.Identity?.Name ?? "Unknown";

            ConnectionMapping.Remove(userId.ToString(), Context.ConnectionId);

            var remainingConnections = ConnectionMapping.GetConnections(userId.ToString());
            if (remainingConnections?.Any() != true)
            {
                await UpdateUserStatus(userId, UserStatus.Offline);
                await Clients.Others.SendAsync(ChatHubEvent.UserOffline, userId);
            }

            await base.OnDisconnectedAsync(exception);
            _logger.LogInformation($"User {userId} ({userName}) disconnected");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnDisconnectedAsync");
            throw;
        }
    }

    public async Task JoinConversation(Guid conversationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var groupId = conversationId.ToString();

            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            await Clients.Group(groupId).SendAsync(ChatHubEvent.UserJoined, userId, conversationId);
            _logger.LogInformation($"User {userId} joined conversation {conversationId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining conversation");
            throw;
        }
    }

    public async Task LeaveConversation(Guid conversationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var groupId = conversationId.ToString();

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
            await Clients.Group(groupId).SendAsync(ChatHubEvent.UserLeft, userId, conversationId);
            _logger.LogInformation($"User {userId} left conversation {conversationId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving conversation");
            throw;
        }
    }

    public async Task SendMessage(MessageRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
            request.SenderId = userId;

            var message = await _chatService.SendMessage(request);
            var groupId = request.ConversationId.ToString();

            await Clients.Group(groupId).SendAsync(ChatHubEvent.ReceiveMessage, message);
            _logger.LogInformation($"Message sent in conversation {request.ConversationId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            throw;
        }
    }

    public async Task SendBroadcast(BroadcastRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var broadcast = await _chatService.CreateBroadcastAsync(
                userId,
                request.Content,
                request.Type,
                request.UserIds,
                request.ScheduledAt
            );

            if (request.ScheduledAt == null)
            {
                foreach (var recipientId in request.UserIds)
                {
                    var connections = ConnectionMapping.GetConnections(recipientId.ToString());
                    if (connections?.Any() == true)
                    {
                        await Clients.Clients(connections)
                            .SendAsync(ChatHubEvent.ReceiveBroadcast, broadcast);
                    }
                }
            }

            await Clients.Caller.SendAsync(ChatHubEvent.BroadcastSent, broadcast.Id);
            _logger.LogInformation($"Broadcast sent to {request.UserIds.Count} users");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending broadcast");
            throw;
        }
    }

    public async Task UpdateMessageStatus(Guid messageId, ReceiptStatus status)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _chatService.UpdateMessageStatus(messageId, userId, status);
            _logger.LogInformation($"Message {messageId} status updated to {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating message status");
            throw;
        }
    }

    public async Task UserTyping(Guid conversationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userName = Context.User?.Identity?.Name ?? "Unknown";
            var groupId = conversationId.ToString();

            await Clients.Group(groupId).SendAsync(
                ChatHubEvent.UserTyping,
                new UserTypingRequestDto
                {
                    UserId = userId,
                    UserName = userName,
                    ConversationId = conversationId
                }
            );

            _logger.LogInformation($"User {userId} is typing in conversation {conversationId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UserStartedTyping");
            throw;
        }
    }

    private Guid GetCurrentUserId()
    {
        var sidClaim = Context.User?.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
        if (string.IsNullOrEmpty(sidClaim))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }
        return Guid.Parse(sidClaim);
    }

    private async Task UpdateUserStatus(Guid userId, UserStatus status)
    {
        try
        {
            // Assuming you have a method to update user status in your service
            // await _chatService.UpdateUserStatus(userId, status);
            await Clients.Others.SendAsync(ChatHubEvent.UserStatusChanged, userId, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user status");
        }
    }
}
