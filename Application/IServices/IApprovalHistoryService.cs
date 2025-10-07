using System.Linq.Expressions;
using Application.Dtos;
using Core.Entities;

namespace Application.IServices;

public interface IApprovalHistoryService
{
    Task<IEnumerable<ApprovalHistoryResponseDto>> GetAllAsync(Guid? approvalInstanceId = null);
    Task<ApiResponseDto<List<ApprovalHistoryResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request, Guid? approvalInstanceId = null);
    Task<ApprovalHistoryResponseDto?> GetByIdAsync(Guid id);
    Task<ApprovalHistoryResponseDto> CreateAsync(ApprovalHistoryRequestDto request);
    Task<ApprovalHistoryResponseDto?> UpdateAsync(Guid id, ApprovalHistoryRequestDto request);
    Task<ApprovalHistoryResponseDto?> UpdateStatusAsync(Guid id, UpdateApprovalActionStatusRequestDto request);
    Task<ApprovalHistoryResponseDto?> UpdateOrderAsync(Guid id, UpdateApprovalActionOrderRequestDto request);
    Task<int> DeleteAsync(Guid id, Guid? deletedBy);
    Task<bool> IsExistAsync(Expression<Func<ApprovalHistory, bool>> predicate);
    Task RebalanceOrdersAsync(Guid approvalInstanceId);
}
