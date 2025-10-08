using System.Linq.Expressions;
using Application.Dtos;
using Core.Entities;

namespace Application.IServices;

public interface IApprovalDocumentService
{
    Task<IEnumerable<ApprovalResponseDto>> GetAllAsync(Guid? approvalFeatureId = null);
    Task<ApiResponseDto<List<ApprovalResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request, Guid? approvalFeatureId = null, Guid? dataId = null);
    Task<ApprovalResponseDto?> GetByIdAsync(Guid id);
    Task<ApprovalResponseDto> CreateAsync(ApprovalDocumentDto request);
    Task<ApprovalResponseDto?> UpdateAsync(Guid id, ApprovalDocumentDto request);
    Task<ApprovalResponseDto?> UpdateStatusAsync(Guid id, UpdateApprovalInstanceStatusRequestDto request);
    Task<ApprovalResponseDto?> UpdateCurrentStepAsync(Guid id, UpdateApprovalInstanceCurrentStepRequestDto request);
    Task<int> DeleteAsync(Guid id, Guid? deletedBy);
    Task<bool> IsExistAsync(Expression<Func<ApprovalDocument, bool>> predicate);
}
