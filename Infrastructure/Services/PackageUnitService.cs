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
        private readonly IGenericRepository<PackageUnit> _repository;
        private readonly IMapper _mapper;

        public PackageUnitService(IGenericRepository<PackageUnit> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PackageUnitResponseDto>> GetAllAsync()
        {
            var entities = await _repository.FindAllAsync();
            return _mapper.Map<IEnumerable<PackageUnitResponseDto>>(entities);
        }

        public async Task<ApiResponseDto<List<PackageUnitResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request)
        {
            var result = await _repository.FindManyWithPagingAsync(request);
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
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            return entity == null ? null : _mapper.Map<PackageUnitResponseDto>(entity);
        }

        public async Task<PackageUnitResponseDto> CreateAsync(PackageUnitRequestDto request)
        {
            var entity = _mapper.Map<PackageUnit>(request);
            entity.Id = Guid.NewGuid();
            await _repository.AddOneAsync(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<PackageUnitResponseDto>(entity);
        }

        public async Task<PackageUnitResponseDto?> UpdateAsync(Guid id, PackageUnitRequestDto request)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;

            entity.Uid = request.Uid;
            entity.Level = request.Level;
            entity.Quantity = request.Quantity;
            entity.UnitId = request.UnitId;

            _repository.UpdateOne(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<PackageUnitResponseDto>(entity);
        }

        public async Task<PackageUnitResponseDto> UpdateStatusAsync(Guid id, PackageUnitStatusUpdateDto request)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id) ?? throw new KeyNotFoundException("PackageUnit not found");
            _repository.UpdateOne(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<PackageUnitResponseDto>(entity);
        }

        public async Task<int> DeleteAsync(Guid id, Guid deletedBy)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return 0;
            _repository.DeleteOne(entity);
            var result = await _repository.SaveChangesAsync();
            return result;
        }

        public async Task<bool> IsExistAsync(Expression<Func<PackageUnit, bool>> predicate)
            => await _repository.FindOneAsync(predicate) != null;
    }
}