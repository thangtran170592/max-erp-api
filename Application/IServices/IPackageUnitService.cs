using System.Linq.Expressions;
using Application.Dtos;
using Core.Entities;

namespace Application.IServices
{
    public interface IPackageUnitService
    {
        Task<IEnumerable<PackageUnitResponseDto>> GetAllAsync();
        Task<ApiResponseDto<List<PackageUnitResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request);
        Task<PackageUnitResponseDto?> GetByIdAsync(Guid id);
        Task<PackageUnitResponseDto> CreateAsync(PackageUnitRequestDto request);
        Task<PackageUnitResponseDto?> UpdateAsync(Guid id, PackageUnitRequestDto request);
        Task<int> DeleteAsync(Guid id, Guid deletedBy);
        Task<PackageUnitResponseDto> UpdateStatusAsync(Guid id, PackageUnitStatusUpdateDto request);
        Task<bool> IsExistAsync(Expression<Func<PackageUnit, bool>> predicate);
    }
}