using Application.Dtos;
using Core.Entities;
using System.Linq.Expressions;

namespace Application.IServices
{
    public interface IApprovalFeatureService
    {
        Task<ApprovalFeatureResponseDto?> GetByIdAsync(Guid id);
        Task<ApprovalFeatureResponseDto?> GetByUidAsync(Guid approvalConfigId, string uid);
        Task<ApiResponseDto<List<ApprovalFeatureResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request, Guid? approvalConfigId = null);
        Task<IEnumerable<ApprovalFeatureResponseDto>> GetAllAsync(Guid? approvalConfigId = null);
        Task<IEnumerable<ApprovalFeatureResponseDto>> GetManyAsync(Expression<Func<ApprovalFeature, bool>> predicate);
        Task<ApprovalFeatureResponseDto> CreateAsync(ApprovalFeatureRequestDto request);
        Task<ApprovalFeatureResponseDto?> UpdateAsync(Guid id, ApprovalFeatureRequestDto request);
        Task<ApprovalFeatureResponseDto?> UpdateStatusAsync(Guid id, UpdateApprovalFeatureStatusRequestDto request);
        Task<int> DeleteAsync(Guid id, Guid? deletedBy);
        Task<bool> IsExistAsync(Expression<Func<ApprovalFeature, bool>> predicate);
    }
}
