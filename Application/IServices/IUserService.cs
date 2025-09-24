using Application.Dtos;

namespace Application.IServices
{
    public interface IUserService
    {
        Task<ApiResponse<List<UserResponseDto>>> FindManyWithPagingAsync(Dictionary<string, object>? filters = null,
            int page = 1,
            int pageSize = 10);
    }
}