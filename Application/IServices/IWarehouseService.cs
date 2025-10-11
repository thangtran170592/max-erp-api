using System.Linq.Expressions;
using Application.Dtos;
using Core.Entities;

namespace Application.IServices
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseResponseDto>> GetAllAsync();
        Task<IEnumerable<WarehouseResponseDto>> GetManyAsync(Expression<Func<Warehouse, bool>> predicate);
        Task<ApiResponseDto<List<WarehouseResponseDto>>> GetManyWithPagingAsync(Guid userId, Guid? departmentId, Guid? positionId, FilterRequestDto request);
        Task<WarehouseResponseDto?> GetByIdAsync(Guid id);
        Task<WarehouseResponseDto> CreateAsync(WarehouseRequestDto request);
        Task<WarehouseResponseDto?> UpdateAsync(Guid id, WarehouseRequestDto request);
        Task<int> DeleteAsync(Guid id, Guid deletedBy);
        Task<WarehouseResponseDto> UpdateStatusAsync(Guid id, UpdateApprovalRequestDto request);
        Task<bool> IsExistAsync(Expression<Func<Warehouse, bool>> predicate);
    }
}