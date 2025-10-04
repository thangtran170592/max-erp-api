using System.Linq.Expressions;
using Application.Dtos;
using Core.Entities;

namespace Application.IServices
{
    public interface IPackageUnitService
    {
        Task<IEnumerable<PackageUnitResponseDto>> GetAllAsync();
        Task<IEnumerable<PackageUnitResponseDto>> GetManyAsync(Expression<Func<PackageUnit, bool>> predicate);
        Task<ApiResponseDto<List<PackageUnitResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request);
        Task<PackageUnitResponseDto?> GetByIdAsync(Guid id);
        Task<PackageUnitResponseDto> CreateAsync(PackageUnitRequestDto request);
        Task<PackageUnitResponseDto?> UpdateAsync(Guid id, PackageUnitRequestDto request);
        Task<int> DeleteAsync(Guid id, Guid deletedBy);
    }
}