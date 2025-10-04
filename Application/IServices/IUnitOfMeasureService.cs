using System.Linq.Expressions;
using Application.Dtos;
using Core.Entities;

namespace Application.IServices
{
    public interface IUnitOfMeasureService
    {
        Task<IEnumerable<UnitOfMeasureResponseDto>> GetAllAsync();
        Task<IEnumerable<UnitOfMeasureResponseDto>> GetManyAsync(Expression<Func<UnitOfMeasure, bool>> predicate);
        Task<ApiResponseDto<List<UnitOfMeasureResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request);
        Task<UnitOfMeasureResponseDto?> GetByIdAsync(Guid id);
        Task<UnitOfMeasureResponseDto> CreateAsync(UnitOfMeasureRequestDto request);
        Task<UnitOfMeasureResponseDto?> UpdateAsync(Guid id, UnitOfMeasureRequestDto request);
        Task<UnitOfMeasureResponseDto> UpdateStatusAsync(Guid id, UnitOfMeasureStatusUpdateDto request);
        Task<int> DeleteAsync(Guid id, Guid deletedBy);
        Task<bool> IsExistAsync(Expression<Func<UnitOfMeasure, bool>> predicate);
    }
}