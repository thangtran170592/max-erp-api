using Application.Dtos;
using Core.Entities;
using System.Linq.Expressions;

namespace Application.IServices
{
    public interface IApprovalConfigService
    {
        Task<ApprovalConfigResponseDto?> GetByIdAsync(Guid id);
        Task<ApprovalConfigResponseDto?> GetByUidAsync(string uid);
        Task<ApiResponseDto<List<ApprovalConfigResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request);
        Task<IEnumerable<ApprovalConfigResponseDto>> GetAllAsync();
        Task<IEnumerable<ApprovalConfigResponseDto>> GetManyAsync(Expression<Func<ApprovalConfig, bool>> predicate);
        Task<ApprovalConfigResponseDto> CreateAsync(ApprovalConfigRequestDto request);
        Task<ApprovalConfigResponseDto?> UpdateAsync(Guid id, ApprovalConfigRequestDto request);
        Task<ApprovalConfigResponseDto?> UpdateStatusAsync(Guid id, UpdateApprovalConfigStatusRequestDto request);
        Task<int> DeleteAsync(Guid id, Guid? deletedBy);
        Task<bool> IsExistAsync(Expression<Func<ApprovalConfig, bool>> predicate);
    }
}
