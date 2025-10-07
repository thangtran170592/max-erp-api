using Application.Dtos;
using Core.Entities;

namespace Application.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAll();
        Task<ApiResponseDto<IEnumerable<UserResponseDto>>> FindManyWithPagingAsync(FilterRequestDto request);
        Task<bool> IsExistAsync(Func<ApplicationUser, bool> predicate);
    }
}