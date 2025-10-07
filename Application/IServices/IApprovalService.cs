using Application.Dtos;

namespace Application.IServices
{
    public interface IApprovalService
    {
    Task<bool> ApproveAsync(ApproveInstanceRequestDto dto);
    }
}