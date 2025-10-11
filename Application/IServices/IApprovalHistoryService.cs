using System.Linq.Expressions;
using Application.Dtos;
using Core.Entities;

namespace Application.IServices;

public interface IApprovalHistoryService
{
    Task<IEnumerable<ApprovalHistoryResponseDto>> GetAllAsync(Guid? approvalInstanceId = null);
    Task<ApiResponseDto<List<ApprovalHistoryResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request, Guid userId, Guid? departmentId, Guid? positionId, Guid? approvalDocumentId);
    Task<ApprovalHistoryResponseDto?> GetByIdAsync(Guid id);
    Task<ApprovalHistoryResponseDto> CreateAsync(ApprovalHistoryRequestDto request);
    Task<int> DeleteAsync(Guid id, Guid? deletedBy);
}
