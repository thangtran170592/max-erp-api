using Application.Dtos;

namespace Application.IServices
{
    public interface IUserService
    {
        Task<ApiResponseDto<List<UserResponseDto>>> FindManyWithPagingAsync(FilterRequestDto request);
    }
}