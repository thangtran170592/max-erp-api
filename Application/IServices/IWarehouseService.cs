using Application.Dtos;

namespace Application.IServices
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseResponseDto>> GetAllAsync();
        Task<WarehouseResponseDto?> GetByIdAsync(Guid id);
        Task<WarehouseResponseDto> CreateAsync(WarehouseRequestDto request);
        Task<WarehouseResponseDto?> UpdateAsync(Guid id, WarehouseRequestDto request);
        Task<bool> DeleteAsync(Guid id);
    }
}