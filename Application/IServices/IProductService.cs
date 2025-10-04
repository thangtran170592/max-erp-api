using System.Linq.Expressions;
using Application.Dtos;
using Core.Entities;

namespace Application.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllAsync();
        Task<IEnumerable<ProductResponseDto>> GetManyAsync(Expression<Func<Product, bool>> predicate);
        Task<ApiResponseDto<List<ProductResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request);
        Task<ProductResponseDto?> GetByIdAsync(Guid id);
        Task<ProductResponseDto> CreateAsync(ProductRequestDto request);
        Task<ProductResponseDto?> UpdateAsync(Guid id, ProductRequestDto request);
        Task<ProductResponseDto> UpdateStatusAsync(Guid id, UpdateProductStatusRequestDto request);
        Task<int> DeleteAsync(Guid id, Guid deletedBy);
        Task<IEnumerable<ProductHistoryResponseDto>> GetProductHistoryAsync(Guid productId);
        Task<bool> IsExistAsync(Expression<Func<Product, bool>> predicate);
    }
}