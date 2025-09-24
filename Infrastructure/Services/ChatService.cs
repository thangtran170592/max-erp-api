using Application.Common.Security;
using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
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
        private readonly IGenericRepository<Message> _messageRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IMapper _mapper;
        public ChatService(
            ApplicationDbContext context,
         UserManager<User> userManager,
         RoleManager<IdentityRole<Guid>> roleManager,
         IGenericRepository<Message> messageRepository,
         IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserResponseDto>> GetChatAbleUsers(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var roles = await _userManager.GetRolesAsync(user!);
            if (roles.Contains(Role.Admin))
            {
                var result = await _userManager.Users.Where(u => u.Id != userId).ToListAsync();
                return result.Select(_mapper.Map<UserResponseDto>);
            }
            var adminRoleId = await _roleManager.Roles
                .Where(r => r.Name == Role.Admin)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();
            var adminUsers = await (from u in _context.Users
                                    join ur in _context.UserRoles on u.Id equals ur.UserId
                                    where ur.RoleId == adminRoleId
                                    select u).ToListAsync();
            return adminUsers.Select(_mapper.Map<UserResponseDto>) ?? [];
        }

        public async Task<IEnumerable<MessageResponseDto>> GetMessages(Guid? roomId, Guid? userId1, Guid? userId2)
        {
            if (roomId.HasValue)
            {
                var result = await _context.Messages.Where(m => m.RoomId == roomId.Value).OrderBy(m => m.CreatedAt).ToListAsync();
                return result.Select(_mapper.Map<MessageResponseDto>);
            }

            if (userId1.HasValue && userId2.HasValue)
            {

                var result = await _context.Messages
                    .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2)
                             || (m.SenderId == userId2 && m.ReceiverId == userId1))
                    .OrderBy(m => m.CreatedAt)
                    .ToListAsync();
                return result.Select(_mapper.Map<MessageResponseDto>);
            }

            return [];
        }

        public async Task<MessageResponseDto?> SendMessage(MessageRequestDto request)
        {
            var message = new Message
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                RoomId = request.RoomId,
                Content = request.Content,
                Type = request.Type
            };
            await _messageRepository.AddAsync(message);
            var resultSave = await _context.SaveChangesAsync();
            return resultSave > 0 ? _mapper.Map<MessageResponseDto>(message) : null;
        }

        public async Task<RoomResponseDto> CreateRoom(string name, Guid adminId, List<Guid> userIds)
        {
            var user = await _userManager.FindByIdAsync(adminId.ToString());
            var roles = await _userManager.GetRolesAsync(user!);
            if (!roles.Contains(Role.Admin))
            {
                throw new Exception("Only admin can create room");
            }

            var room = new Room { Name = name, CreatedBy = adminId };
            _context.Rooms.Add(room);

            _context.RoomUsers.Add(new RoomUser { RoomId = room.Id, UserId = adminId });

            foreach (var uid in userIds)
            {
                _context.RoomUsers.Add(new RoomUser { RoomId = room.Id, UserId = uid });
            }

            await _context.SaveChangesAsync();
            return _mapper.Map<RoomResponseDto>(room);
        }

        public async Task<IEnumerable<RoomResponseDto>> GetUserRooms(Guid userId)
        {
            var result = await _context.Rooms
                .Include(r => r.RoomUsers)
                .Where(r => r.RoomUsers.Any(ru => ru.UserId == userId))
                .ToListAsync();
            return result.Select(_mapper.Map<RoomResponseDto>);
        }

        public async Task<int> SendBroadcast(Guid senderId, string content, MessageType type, string targetType, Guid? targetId)
        {
            var broadcast = new Broadcast { SenderId = senderId, Content = content, Type = type, TargetType = targetType, TargetId = targetId };
            _context.Broadcasts.Add(broadcast);

            List<Guid> userIds = targetType switch
            {
                "All" => await _context.Users.Where(u => u.Id != senderId).Select(u => u.Id).ToListAsync(),
                "Group" => await _context.RoomUsers.Where(r => r.RoomId == targetId).Select(r => r.UserId).ToListAsync(),
                _ => new List<Guid>()
            };

            foreach (var uid in userIds)
            {
                _context.Messages.Add(new Message { SenderId = senderId, ReceiverId = uid, Content = content, Type = type });
            }

            return await _context.SaveChangesAsync();
        }
    }
}