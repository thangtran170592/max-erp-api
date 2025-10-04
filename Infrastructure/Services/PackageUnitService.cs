using System.Linq.Expressions;
using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Core.Entities;

namespace Infrastructure.Services
{
    public class PackageUnitService : IPackageUnitService
    {
        private readonly IGenericRepository<PackageUnit> _repositoryPackageUnit;
        private readonly IMapper _mapper;

        public PackageUnitService(IGenericRepository<PackageUnit> repository, IMapper mapper)
        {
            _repositoryPackageUnit = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PackageUnitResponseDto>> GetAllAsync()
        {
            Expression<Func<PackageUnit, object>>[] includes = [static x => x.Unit!, static x => x.Package!];
            var entities = await _repositoryPackageUnit.FindAllAsync(includes: includes);
            return _mapper.Map<IEnumerable<PackageUnitResponseDto>>(entities);
        }

        public async Task<IEnumerable<PackageUnitResponseDto>> GetManyAsync(Expression<Func<PackageUnit, bool>> predicate)
        {
            Expression<Func<PackageUnit, object>>[] includes = [static x => x.Unit!, static x => x.Package!];
            var entities = await _repositoryPackageUnit.FindManyAsync(predicate, includes);
            return _mapper.Map<IEnumerable<PackageUnitResponseDto>>(entities);
        }

        public async Task<ApiResponseDto<List<PackageUnitResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request)
        {
            Expression<Func<PackageUnit, object>>[] includes = [static x => x.Unit!, static x => x.Package!];
            var result = await _repositoryPackageUnit.FindManyWithPagingAsync(request, includes);
            return new ApiResponseDto<List<PackageUnitResponseDto>>
            {
                Data = _mapper.Map<List<PackageUnitResponseDto>>(result.Data),
                PageData = result.PageData,
                Message = result.Message,
                Success = result.Success,
                Timestamp = result.Timestamp,
            };
        }

        public async Task<PackageUnitResponseDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repositoryPackageUnit.FindOneAsync(x => x.Id == id);
            return entity == null ? null : _mapper.Map<PackageUnitResponseDto>(entity);
        }

        public async Task<PackageUnitResponseDto> CreateAsync(PackageUnitRequestDto request)
        {
            var entity = _mapper.Map<PackageUnit>(request);
            entity.Id = Guid.NewGuid();
            await _repositoryPackageUnit.AddOneAsync(entity);
            await _repositoryPackageUnit.SaveChangesAsync();
            Expression<Func<PackageUnit, object>>[] includes = [static x => x.Unit!, static x => x.Package!];
            var result = await _repositoryPackageUnit.FindOneAsync(x => x.Id == entity.Id, includes);
            return _mapper.Map<PackageUnitResponseDto>(result);
        }

        public async Task<PackageUnitResponseDto?> UpdateAsync(Guid id, PackageUnitRequestDto request)
        {
            var entity = await _repositoryPackageUnit.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;

            entity.PackageId = request.PackageId;
            entity.Level = request.Level;
            entity.Quantity = request.Quantity;
            entity.UnitId = request.UnitId;

            _repositoryPackageUnit.UpdateOne(entity);
            await _repositoryPackageUnit.SaveChangesAsync();
            Expression<Func<PackageUnit, object>>[] includes = [static x => x.Unit!, static x => x.Package!];
            var result = await _repositoryPackageUnit.FindOneAsync(x => x.Id == entity.Id, includes);
            return _mapper.Map<PackageUnitResponseDto>(result);
        }

        public async Task<int> DeleteAsync(Guid id, Guid deletedBy)
        {
            var entity = await _repositoryPackageUnit.FindOneAsync(x => x.Id == id);
            if (entity == null) return 0;
            _repositoryPackageUnit.DeleteOne(entity);
            await _repositoryPackageUnit.SaveChangesAsync();
            return 1;
        }
    }
}