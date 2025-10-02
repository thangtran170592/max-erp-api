using System.Linq.Expressions;
using Application.Dtos;
using Core.Entities;

namespace Application.IServices
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierResponseDto>> GetAllAsync();
        Task<ApiResponseDto<List<SupplierResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request);
        Task<SupplierResponseDto?> GetByIdAsync(Guid id);
        Task<SupplierResponseDto> CreateAsync(SupplierRequestDto request);
        Task<SupplierResponseDto?> UpdateAsync(Guid id, SupplierRequestDto request);
        Task<SupplierResponseDto> UpdateStatusAsync(Guid id, SupplierStatusUpdateDto request);
        Task<int> DeleteAsync(Guid id, Guid deletedBy);
        Task<bool> IsExistAsync(Expression<Func<Supplier, bool>> predicate);
    }
}