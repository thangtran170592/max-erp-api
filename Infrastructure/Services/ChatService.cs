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

            // Lấy thông tin user cho từng member
            var users = await _userManager.Users
                .Where(u => request.MemberIds.Contains(u.Id))
                .ToListAsync();

            var members = users.Select(user => new ConversationMember
            {
                ConversationId = conversation.Id,
                UserId = user.Id,
                User = user,
                Role = request.Type == ConversationType.Group && user.Id == request.CreatorId ? ChatRole.Owner : ChatRole.Member
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
            // Lấy thông tin user hiện tại và vai trò
            var currentUser = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new UnauthorizedAccessException("User not found");

            var userRoles = await _userManager.GetRolesAsync(currentUser);
            var isAdmin = userRoles.Contains(Role.Admin.ToString());

            // Lấy các conversation đã tồn tại của user hiện tại
            var existingConversations = await _context.Conversations
                .Include(c => c.Members)
                .Where(c => c.Members.Any(u => u.UserId == userId))
                .Where(c => c.Type == ConversationType.Private || c.Type == ConversationType.Group)
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();

            List<User> targetUsers;
            if (isAdmin)
            {
                // Admin: lấy tất cả user khác trừ chính mình
                targetUsers = await _userManager.Users
                    .Where(u => u.Id != userId)
                    .AsNoTracking()
                    .ToListAsync();
            }
            else
            {
                // Không phải admin: lấy tất cả admin khác đã hoặc chưa chat với user này
                var adminUsers = await _userManager.GetUsersInRoleAsync(Role.Admin.ToString());
                targetUsers = adminUsers
                    .Where(u => u.Id != userId)
                    .ToList();
            }

            // Lấy các user đã có conversation riêng với user hiện tại
            var existingPrivateUserIds = existingConversations
                .Where(c => c.Type == ConversationType.Private)
                .SelectMany(c => c.Members)
                .Where(m => m.UserId != userId)
                .Select(m => m.UserId)
                .ToHashSet();

            // Tạo virtual conversation cho các user chưa có chat riêng
            var virtualConversations = targetUsers
                .Where(u => !existingPrivateUserIds.Contains(u.Id))
                .Select(user => new Conversation
                {
                    Id = Guid.Empty,
                    Type = ConversationType.Private,
                    Name = string.Empty,
                    Members = new List<ConversationMember>
                    {
                new() { ConversationId = Guid.Empty, UserId = currentUser.Id, Role = ChatRole.Member, User = currentUser },
                new() { ConversationId = Guid.Empty, UserId = user.Id, Role = ChatRole.Member, User = user }
                    }
                });

            // Gộp conversation thật và conversation ảo
            var allConversations = existingConversations.Concat(virtualConversations);

            return _mapper.Map<IEnumerable<ConversationResponseDto>>(allConversations);
        }
    }
}