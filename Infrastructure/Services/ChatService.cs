using Application.Common.Helpers;
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
        private readonly IGenericRepository<Room> _roomRepository;
        private readonly IGenericRepository<RoomUser> _roomUserRepository;
        private readonly IGenericRepository<Broadcast> _broadcastRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IMapper _mapper;
        public ChatService(
            ApplicationDbContext context,
         UserManager<User> userManager,
         RoleManager<IdentityRole<Guid>> roleManager,
         IGenericRepository<Message> messageRepository,
         IGenericRepository<Room> roomRepository,
         IGenericRepository<RoomUser> roomUserRepository,
         IGenericRepository<Broadcast> broadcastRepository,
         IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _messageRepository = messageRepository;
            _roomRepository = roomRepository;
            _roomUserRepository = roomUserRepository;
            _broadcastRepository = broadcastRepository;
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
                var result = await _messageRepository.FindManyAsync(m => m.RoomId == roomId.Value);
                return result.OrderBy(m => m.CreatedAt).Select(_mapper.Map<MessageResponseDto>);
            }

            if (userId1.HasValue && userId2.HasValue)
            {
                var result = await _messageRepository.FindManyAsync(m => (m.SenderId == userId1 && m.ReceiverId == userId2)
                             || (m.SenderId == userId2 && m.ReceiverId == userId1));

                return result.OrderBy(m => m.CreatedAt).Select(_mapper.Map<MessageResponseDto>);
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

        public async Task<RoomResponseDto?> ValidateRoom(string roomCode, bool isGroup)
        {
            var room = await _roomRepository.FindOneAsync(x => x.RoomCode.Equals(roomCode) && x.IsGroup == isGroup);
            return room is not null ? _mapper.Map<RoomResponseDto>(room) : null;
        }

        public async Task<RoomResponseDto> CreateRoom(RoomRequestDto request)
        {
            var user = await _userManager.FindByIdAsync(request.HostId);
            var roles = await _userManager.GetRolesAsync(user!);
            if (!roles.Contains(Role.Admin))
            {
                throw new Exception("Only admin can create room");
            }

            var room = _mapper.Map<Room>(request);
            var hostId = Guid.Parse(request.HostId);
            room.RoomCode = ChatHubHelper.SetRoomCode(hostId, request.UserIds[0], request.Name, request.IsGroup);

            await _roomRepository.AddAsync(room);

            await _roomUserRepository.AddAsync(new RoomUser { RoomId = room.Id, UserId = hostId });

            var roomUsers = new List<RoomUser>();
            roomUsers.AddRange(request.UserIds.Select(u => new RoomUser { RoomId = room.Id, UserId = u }));

            await _roomUserRepository.AddRangeAsync(roomUsers);

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

        public async Task<List<Guid>> SendBroadcast(BroadcastRequestDto request)
        {
            var broadcast = _mapper.Map<Broadcast>(request);
            await _broadcastRepository.AddAsync(broadcast);

            List<Guid> userIds = request.TargetType switch
            {
                TargetType.All => await _context.Users.Where(user => user.Id != request.SenderId).Select(user => user.Id).ToListAsync(),
                TargetType.Group => await _context.RoomUsers.Where(room => room.RoomId == request.TargetId).Select(room => room.UserId).ToListAsync(),
                _ => []
            };

            IEnumerable<Message> messages = userIds.Select(uid =>
            new Message
            {
                SenderId = request.SenderId,
                ReceiverId = uid,
                Content = request.Content,
                Type = request.Type,
                RoomId = request.TargetType == TargetType.Group ? request.TargetId : null,
                SenderName = request.SenderName,
            });
            await _messageRepository.AddRangeAsync(messages);
            await _context.SaveChangesAsync();
            return userIds;
        }
    }
}