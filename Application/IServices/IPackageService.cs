using System.Linq.Expressions;
using Application.Dtos;
using Core.Entities;

namespace Application.IServices
{
    public interface IPackageService
    {
        Task<IEnumerable<PackageResponseDto>> GetAllAsync();
        Task<IEnumerable<PackageResponseDto>> GetManyAsync(Expression<Func<Package, bool>> predicate);
        Task<ApiResponseDto<List<PackageResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request);
        Task<PackageResponseDto?> GetByIdAsync(Guid id);
        Task<PackageResponseDto> CreateAsync(PackageRequestDto request);
        Task<PackageResponseDto?> UpdateAsync(Guid id, PackageRequestDto request);
        Task<PackageResponseDto> UpdateStatusAsync(Guid id, PackageStatusUpdateDto request);
        Task<int> DeleteAsync(Guid id, Guid deletedBy);
        Task<bool> IsExistAsync(Expression<Func<Package, bool>> predicate);
    }
}