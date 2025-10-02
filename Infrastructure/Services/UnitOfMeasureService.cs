using System.Linq.Expressions;
using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Core.Entities;

namespace Infrastructure.Services
{
    public class UnitOfMeasureService : IUnitOfMeasureService
    {
        private readonly IGenericRepository<UnitOfMeasure> _repository;
        private readonly IMapper _mapper;

        public UnitOfMeasureService(IGenericRepository<UnitOfMeasure> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UnitOfMeasureResponseDto>> GetAllAsync()
        {
            var entities = await _repository.FindAllAsync();
            return _mapper.Map<IEnumerable<UnitOfMeasureResponseDto>>(entities);
        }

        public async Task<ApiResponseDto<List<UnitOfMeasureResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request)
        {
            var result = await _repository.FindManyWithPagingAsync(request);
            return new ApiResponseDto<List<UnitOfMeasureResponseDto>>
            {
                Data = _mapper.Map<List<UnitOfMeasureResponseDto>>(result.Data),
                PageData = result.PageData,
                Message = result.Message,
                Success = result.Success,
                Timestamp = result.Timestamp,
            };
        }

        public async Task<UnitOfMeasureResponseDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            return entity == null ? null : _mapper.Map<UnitOfMeasureResponseDto>(entity);
        }

        public async Task<UnitOfMeasureResponseDto> CreateAsync(UnitOfMeasureRequestDto request)
        {
            var entity = _mapper.Map<UnitOfMeasure>(request);
            entity.Id = Guid.NewGuid();
            await _repository.AddOneAsync(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<UnitOfMeasureResponseDto>(entity);
        }

        public async Task<UnitOfMeasureResponseDto?> UpdateAsync(Guid id, UnitOfMeasureRequestDto request)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;

            entity.Name = request.Name;
            entity.Status = request.Status;

            _repository.UpdateOne(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<UnitOfMeasureResponseDto>(entity);
        }

        public async Task<UnitOfMeasureResponseDto> UpdateStatusAsync(Guid id, UnitOfMeasureStatusUpdateDto request)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id) ?? throw new KeyNotFoundException("UnitOfMeasure not found");
            entity.Status = request.Status;
            _repository.UpdateOne(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<UnitOfMeasureResponseDto>(entity);
        }

        public async Task<int> DeleteAsync(Guid id, Guid deletedBy)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return 0;
            _repository.DeleteOne(entity);
            var result = await _repository.SaveChangesAsync();
            return result;
        }

        public async Task<bool> IsExistAsync(Expression<Func<UnitOfMeasure, bool>> predicate)
            => await _repository.FindOneAsync(predicate) != null;
    }
}