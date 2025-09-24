using Application.Common.Helpers;
using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Core.Entities;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;
        public UserService(
            IGenericRepository<User> userRepository,
             IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<UserResponseDto>>> FindManyWithPagingAsync(
            Dictionary<string, object>? filters = null,
            int page = 1,
            int pageSize = 10)
        {
            var pagedResult = await _userRepository.FindManyWithPagingAsync(filters, page, pageSize);
            var result = ApiResponseHelper.CreateSuccessResponse<User, UserResponseDto>(pagedResult, _mapper);
            return result;
        }
    }
}