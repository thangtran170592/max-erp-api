using Application.Dtos;
using Core.Entities;
using System.Linq.Expressions;

namespace Application.IServices
{
    public interface IApprovalStepService
    {
        Task<ApprovalStepResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ApprovalStepResponseDto>> GetAllAsync(Guid? featureId = null);
        Task<ApprovalStepResponseDto> CreateAsync(ApprovalStepRequestDto request);
        Task<ApprovalStepResponseDto?> UpdateAsync(Guid id, ApprovalStepRequestDto request);
        Task<ApprovalStepResponseDto?> UpdateOrderAsync(Guid id, UpdateApprovalStepOrderRequestDto request);
        Task<ApprovalStepResponseDto?> UpdateIsFinalAsync(Guid id, UpdateApprovalStepStatusRequestDto request);
        Task<int> DeleteAsync(Guid id, Guid? deletedBy);
        Task RebalanceOrdersAsync(Guid approvalFeatureId);
    }
}
