using Application.Dtos;

namespace Application.IServices
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseDto>> GetAllAsync();
        Task<WarehouseDto?> GetByIdAsync(Guid id);
        Task<WarehouseDto> CreateAsync(WarehouseDto request);
        Task<WarehouseDto?> UpdateAsync(Guid id, WarehouseDto request);
        Task<bool> DeleteAsync(Guid id);
    }
}