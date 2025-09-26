using Application.Common.Security;
using Application.Dtos;
using Application.IServices;
using AutoMapper;
using Core.Constants;
using Core.Entities;
using Core.Enums;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public ChatService(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<MessageResponseDto> SendMessage(MessageRequestDto request)
        {
            var message = new Message
            {
                ConversationId = request.ConversationId,
                SenderId = request.SenderId,
                Content = request.Content,
                Type = request.Type,
                Status = MessageStatus.Sent
            };

            _context.Messages.Add(message);

            // Create receipts for all conversation members except sender
            var members = await _context.ConversationMembers
                .Where(m => m.ConversationId == request.ConversationId && m.UserId != request.SenderId)
                .ToListAsync();

            foreach (var member in members)
            {
                _context.MessageReceipts.Add(new MessageReceipt
                {
                    MessageId = message.Id,
                    UserId = member.UserId,
                    Status = ReceiptStatus.Delivered
                });
            }

            // Update conversation's last message
            var conversation = await _context.Conversations.FindAsync(request.ConversationId);
            if (conversation != null)
            {
                conversation.LastMessage = request.Content;
                conversation.LastMessageAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return _mapper.Map<MessageResponseDto>(message);
        }

        public async Task<ConversationResponseDto> CreateConversation(ConversationRequestDto request)
        {
            var conversation = new Conversation
            {
                Name = request.Name,
                Type = request.Type
            };

            _context.Conversations.Add(conversation);

            // Add members
            var members = request.MemberIds.Select(userId => new ConversationMember
            {
                ConversationId = conversation.Id,
                UserId = userId,
                Role = userId == request.CreatorId ? ChatRole.Owner : ChatRole.Member
            });

            await _context.ConversationMembers.AddRangeAsync(members);
            await _context.SaveChangesAsync();

            return _mapper.Map<ConversationResponseDto>(conversation);
        }

        public async Task<BroadcastResponseDto> CreateBroadcastAsync(
            Guid senderId,
            string content,
            MessageType type,
            List<Guid> userIds,
            DateTime? scheduledAt = null)
        {
            var broadcast = new Broadcast
            {
                SenderId = senderId,
                Content = content,
                Type = type,
                IsScheduled = scheduledAt.HasValue,
                ScheduledAt = scheduledAt,
                Status = scheduledAt.HasValue ? BroadcastStatus.Scheduled : BroadcastStatus.Sent
            };

            _context.Broadcasts.Add(broadcast);

            // Create recipients
            var recipients = userIds.Select(userId => new BroadcastRecipient
            {
                BroadcastId = broadcast.Id,
                UserId = userId,
                Status = ReceiptStatus.Delivered
            });

            await _context.BroadcastRecipients.AddRangeAsync(recipients);
            await _context.SaveChangesAsync();

            return _mapper.Map<BroadcastResponseDto>(broadcast);
        }

        public async Task<IEnumerable<MessageResponseDto>> GetConversationMessages(
            Guid conversationId,
            int skip = 0,
            int take = 50)
        {
            var messages = await _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Include(m => m.Sender)
                .Include(m => m.Receipts)
                .ToListAsync();

            return messages.Select(_mapper.Map<MessageResponseDto>);
        }

        public async Task UpdateMessageStatus(Guid messageId, Guid userId, ReceiptStatus status)
        {
            var receipt = await _context.MessageReceipts
                .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == userId);

            if (receipt != null)
            {
                receipt.Status = status;
                receipt.ReadAt = status == ReceiptStatus.Read ? DateTime.UtcNow : null;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ConversationResponseDto>> GetUserConversations(Guid userId)
        {
            // Get current user with roles
            var currentUser = await _userManager.FindByIdAsync(userId.ToString());
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not found");

            var userRoles = await _userManager.GetRolesAsync(currentUser);

            // Get base query for conversations
            var conversationsQuery = _context.Conversations
                .Include(c => c.Members)
                .ThenInclude(m => m.User)
                .Where(c => c.Members.Any(m => m.UserId == userId));

            // Get existing private conversations
            var existingConversations = await conversationsQuery
                .Where(c => c.Type == ConversationType.Private || c.Type == ConversationType.Group)
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();

            // Get potential users for private chat based on roles
            var potentialUsersQuery = _userManager.Users.Where(u => u.Id != userId);

            if (userRoles.Contains("Admin"))
            {
                // Admins can see all users except themselves
                potentialUsersQuery = potentialUsersQuery.Where(u => u.Id != userId);
            }
            else
            {
                // Non-admins can only see admins
                var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                var adminIds = adminUsers.Select(u => u.Id);
                potentialUsersQuery = potentialUsersQuery.Where(u => adminIds.Contains(u.Id));
            }

            var potentialUsers = await potentialUsersQuery.ToListAsync();

            // Create virtual conversations for potential private chats
            var virtualConversations = potentialUsers.Select(user => new Conversation
            {
                Id = Guid.Empty,
                Type = ConversationType.Private,
                Name = user.FullName ?? user.UserName!,
                Members =
                [
                    new() { ConversationId = Guid.Empty, UserId = currentUser.Id, Role = ChatRole.Owner, User = currentUser },
                    new() { ConversationId = Guid.Empty, UserId = user.Id, Role = ChatRole.Member, User = user }
                ]
            });
            
            // Combine existing and potential conversations
            var allConversations = existingConversations.Concat(virtualConversations);

            // Filter out virtual conversations that already exist
            var existingPrivateUserIds = existingConversations
                .Where(c => c.Type == ConversationType.Private)
                .SelectMany(c => c.Members)
                .Where(m => m.UserId != userId)
                .Select(m => m.UserId);

            var filteredConversations = allConversations
                .Where(c => c.Type != ConversationType.Private ||
                            !existingPrivateUserIds.Contains(c.Members.First(m => m.UserId != userId).UserId));

            return _mapper.Map<IEnumerable<ConversationResponseDto>>(filteredConversations);
        }
    }
}