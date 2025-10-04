//// filepath: /Users/kevin/Documents/Projects/MiniStore/Infrastructure/Services/SupplierService.cs
using System.Linq.Expressions;
using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Core.Entities;

namespace Infrastructure.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IGenericRepository<Supplier> _repositorySupplier;
        private readonly IMapper _mapper;

        public SupplierService(IGenericRepository<Supplier> repositorySupplier, IMapper mapper)
        {
            _repositorySupplier = repositorySupplier;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SupplierResponseDto>> GetAllAsync()
        {
            var entities = await _repositorySupplier.FindAllAsync();
            return _mapper.Map<IEnumerable<SupplierResponseDto>>(entities);
        }

        public async Task<IEnumerable<SupplierResponseDto>> GetManyAsync(Expression<Func<Supplier, bool>> predicate)
        {
            var entities = await _repositorySupplier.FindManyAsync(predicate);
            return _mapper.Map<IEnumerable<SupplierResponseDto>>(entities);
        }

        public async Task<ApiResponseDto<List<SupplierResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request)
        {
            var result = await _repositorySupplier.FindManyWithPagingAsync(request);
            return new ApiResponseDto<List<SupplierResponseDto>>
            {
                Data = _mapper.Map<List<SupplierResponseDto>>(result.Data),
                PageData = result.PageData,
                Message = result.Message,
                Success = result.Success,
                Timestamp = result.Timestamp,
            };
        }

        public async Task<SupplierResponseDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repositorySupplier.FindOneAsync(x => x.Id == id);
            return entity == null ? null : _mapper.Map<SupplierResponseDto>(entity);
        }

        public async Task<SupplierResponseDto> CreateAsync(SupplierRequestDto request)
        {
            var entity = _mapper.Map<Supplier>(request);
            entity.Id = Guid.NewGuid();
            await _repositorySupplier.AddOneAsync(entity);
            await _repositorySupplier.SaveChangesAsync();
            return _mapper.Map<SupplierResponseDto>(entity);
        }

        public async Task<SupplierResponseDto?> UpdateAsync(Guid id, SupplierRequestDto request)
        {
            var entity = await _repositorySupplier.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;

            entity.Name = request.Name;
            entity.Address = request.Address;
            entity.Tax = request.Tax;
            entity.Email = request.Email;
            entity.Status = request.Status;

            _repositorySupplier.UpdateOne(entity);
            await _repositorySupplier.SaveChangesAsync();
            return _mapper.Map<SupplierResponseDto>(entity);
        }

        public async Task<SupplierResponseDto> UpdateStatusAsync(Guid id, SupplierStatusUpdateDto request)
        {
            var entity = await _repositorySupplier.FindOneAsync(x => x.Id == id)
                ?? throw new KeyNotFoundException("Supplier not found");
            entity.Status = request.Status;
            _repositorySupplier.UpdateOne(entity);
            await _repositorySupplier.SaveChangesAsync();
            return _mapper.Map<SupplierResponseDto>(entity);
        }

        public async Task<int> DeleteAsync(Guid id, Guid deletedBy)
        {
            var entity = await _repositorySupplier.FindOneAsync(x => x.Id == id);
            if (entity == null) return 0;
            _repositorySupplier.DeleteOne(entity);
            var result = await _repositorySupplier.SaveChangesAsync();
            return result;
        }

        public async Task<bool> IsExistAsync(Expression<Func<Supplier, bool>> predicate)
            => await _repositorySupplier.FindOneAsync(predicate) != null;
    }
}