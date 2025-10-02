using System.Linq.Expressions;
using Application.Dtos;
using Core.Entities;

namespace Application.IServices
{
    public interface IProductCategoryService
    {
        Task<IEnumerable<ProductCategoryResponseDto>> GetAllAsync();
        Task<ApiResponseDto<List<ProductCategoryResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request);
        Task<ProductCategoryResponseDto?> GetByIdAsync(Guid id);
        Task<ProductCategoryResponseDto> CreateAsync(ProductCategoryRequestDto request);
        Task<ProductCategoryResponseDto?> UpdateAsync(Guid id, ProductCategoryRequestDto request);
        Task<ProductCategoryResponseDto> UpdateStatusAsync(Guid id, ProductCategoryStatusUpdateDto request);
        Task<int> DeleteAsync(Guid id, Guid deletedBy);
        Task<bool> IsExistAsync(Expression<Func<ProductCategory, bool>> predicate);
    }
}