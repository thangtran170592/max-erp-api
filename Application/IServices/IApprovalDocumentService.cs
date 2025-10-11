using System.Linq.Expressions;
using Application.Dtos;
using Core.Entities;

namespace Application.IServices;

public interface IApprovalDocumentService
{
    Task<IEnumerable<ApprovalDocumentResponseDto>> GetAllAsync(Guid? approvalFeatureId = null);
    Task<ApiResponseDto<List<ApprovalDocumentResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request, Guid? approvalFeatureId = null, Guid? dataId = null);
    Task<ApprovalDocumentResponseDto?> GetByIdAsync(Guid id);
    Task<ApprovalDocumentResponseDto> CreateAsync(ApprovalDocumentDto request);
    Task<ApprovalDocumentResponseDto?> UpdateAsync(Guid id, ApprovalDocumentDto request);
    Task<ApprovalDocumentResponseDto?> UpdateStatusAsync(Guid id, UpdateApprovalDocumentStatusRequestDto request);
    Task<ApprovalDocumentResponseDto?> UpdateCurrentStepAsync(Guid id, UpdateApprovalDocumentCurrentStepRequestDto request);
    Task<int> DeleteAsync(Guid id, Guid? deletedBy);
    Task<bool> IsExistAsync(Expression<Func<ApprovalDocument, bool>> predicate);
}
