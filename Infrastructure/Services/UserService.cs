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
        private readonly IGenericRepository<ApplicationUser> _userRepository;
        private readonly IMapper _mapper;
        public UserService(
            IGenericRepository<ApplicationUser> userRepository,
             IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<List<UserResponseDto>>> FindManyWithPagingAsync(FilterRequestDto request)
        {
            var result = await _userRepository.FindManyWithPagingAsync(request);
            var response = ApiResponseHelper.CreateSuccessResponse<ApplicationUser, UserResponseDto>(result, _mapper);
            return response;
        }
    }
}