using System.Linq.Expressions;
using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Core.Entities;

namespace Infrastructure.Services
{
    public class PackageService : IPackageService
    {
        private readonly IGenericRepository<Package> _repositoryPackage;
        private readonly IMapper _mapper;

        public PackageService(IGenericRepository<Package> repositoryPackage, IMapper mapper)
        {
            _repositoryPackage = repositoryPackage;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PackageResponseDto>> GetAllAsync()
        {
            var entities = await _repositoryPackage.FindAllAsync();
            return _mapper.Map<IEnumerable<PackageResponseDto>>(entities);
        }

        public async Task<IEnumerable<PackageResponseDto>> GetManyAsync(Expression<Func<Package, bool>> predicate)
        {
            var entities = await _repositoryPackage.FindManyAsync(predicate);
            return _mapper.Map<IEnumerable<PackageResponseDto>>(entities);
        }

        public async Task<ApiResponseDto<List<PackageResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request)
        {
            var result = await _repositoryPackage.FindManyWithPagingAsync(request);

            return new ApiResponseDto<List<PackageResponseDto>>
            {
                Data = _mapper.Map<List<PackageResponseDto>>(result.Data),
                PageData = result.PageData,
                Message = result.Message,
                Success = result.Success,
                Timestamp = result.Timestamp,
            };
        }

        public async Task<PackageResponseDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repositoryPackage.FindOneAsync(x => x.Id == id);
            return entity == null ? null : _mapper.Map<PackageResponseDto>(entity);
        }

        public async Task<PackageResponseDto> CreateAsync(PackageRequestDto request)
        {
            var entity = _mapper.Map<Package>(request);
            entity.Id = Guid.NewGuid();
            await _repositoryPackage.AddOneAsync(entity);
            await _repositoryPackage.SaveChangesAsync();
            return _mapper.Map<PackageResponseDto>(entity);
        }

        public async Task<PackageResponseDto?> UpdateAsync(Guid id, PackageRequestDto request)
        {
            var entity = await _repositoryPackage.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;

            entity.Name = request.Name;
            entity.Status = request.Status;

            _repositoryPackage.UpdateOne(entity);
            await _repositoryPackage.SaveChangesAsync();
            return _mapper.Map<PackageResponseDto>(entity);
        }

        public async Task<PackageResponseDto> UpdateStatusAsync(Guid id, PackageStatusUpdateDto request)
        {
            var entity = await _repositoryPackage.FindOneAsync(x => x.Id == id)
                ?? throw new KeyNotFoundException("Package not found");
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = request.UpdatedBy;
            entity.Status = request.Status;
            _repositoryPackage.UpdateOne(entity);
            await _repositoryPackage.SaveChangesAsync();
            return _mapper.Map<PackageResponseDto>(entity);
        }

        public async Task<int> DeleteAsync(Guid id, Guid deletedBy)
        {
            var entity = await _repositoryPackage.FindOneAsync(x => x.Id == id);
            if (entity == null) return 0;
            _repositoryPackage.DeleteOne(entity);
            await _repositoryPackage.SaveChangesAsync();
            return 1;
        }

        public async Task<bool> IsExistAsync(Expression<Func<Package, bool>> predicate)
            => await _repositoryPackage.FindOneAsync(predicate) != null;
    }
}