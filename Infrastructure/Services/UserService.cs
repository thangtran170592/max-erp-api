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

        public async Task<IEnumerable<UserResponseDto>> GetAll()
        {
            var result = await _userRepository.FindAllAsync();
            return _mapper.Map<IEnumerable<UserResponseDto>>(result);
        }

        public async Task<ApiResponseDto<IEnumerable<UserResponseDto>>> FindManyWithPagingAsync(FilterRequestDto request)
        {
            var result = await _userRepository.FindManyWithPagingAsync(request);
            return new ApiResponseDto<IEnumerable<UserResponseDto>>
            {
                Data = _mapper.Map<IEnumerable<UserResponseDto>>(result.Data),
                PageData = result.PageData,
                Message = result.Message,
                Success = result.Success,
                Timestamp = result.Timestamp,
            };
        }

        public async Task<bool> IsExistAsync(Func<ApplicationUser, bool> predicate)
        {
            return await _userRepository.FindOneAsync(x => predicate(x)) != null;
        }
    }
}