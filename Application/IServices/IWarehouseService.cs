using Application.Common.Models;
using Application.Dtos;
using Core.Enums;

namespace Application.IServices
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseResponseDto>> GetAllAsync();
        Task<ApiResponseDto<List<WarehouseResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request);
        Task<WarehouseResponseDto?> GetByIdAsync(Guid id);
        Task<WarehouseResponseDto> CreateAsync(WarehouseRequestDto request);
        Task<WarehouseResponseDto?> UpdateAsync(Guid id, WarehouseRequestDto request);
        Task<int> DeleteAsync(Guid id, Guid deletedBy);
        Task<WarehouseResponseDto> UpdateStatusAsync(Guid id, WarehouseStatusUpdateDto request);
        Task<IEnumerable<WarehouseHistoryDto>> GetWarehouseHistoryAsync(Guid warehouseId);
    }
}