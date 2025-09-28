using Application.Dtos;

namespace Application.IServices
{
    public interface IWarehouseHistoryService
    {
        Task<IEnumerable<WarehouseHistoryDto>> GetAllAsync();
        Task<WarehouseHistoryDto?> GetByIdAsync(Guid id);
        Task<WarehouseHistoryDto> CreateAsync(WarehouseHistoryDto request);
        Task<WarehouseHistoryDto?> UpdateAsync(Guid id, WarehouseHistoryDto request);
        Task<bool> DeleteAsync(Guid id);
    }
}