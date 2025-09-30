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

        public async Task<ApiResponseDto<List<UserResponseDto>>> FindManyWithPagingAsync(FilterRequestDto request)
        {
            var result = await _userRepository.FindManyWithPagingAsync(request);
            var response = ApiResponseHelper.CreateSuccessResponse<User, UserResponseDto>(result, _mapper);
            return response;
        }
    }
}