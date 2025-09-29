using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Core.Entities;

namespace Infrastructure.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IGenericRepository<Warehouse> _repository;
        private readonly IMapper _mapper;

        public WarehouseService(IGenericRepository<Warehouse> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WarehouseResponseDto>> GetAllAsync()
        {
            var warehouses = await _repository.FindAllAsync();
            return _mapper.Map<IEnumerable<WarehouseResponseDto>>(warehouses);
        }

        public async Task<WarehouseResponseDto?> GetByIdAsync(Guid id)
        {
            var warehouse = await _repository.FindOneAsync(x => x.Id == id);
            return warehouse == null ? null : _mapper.Map<WarehouseResponseDto>(warehouse);
        }

        public async Task<WarehouseResponseDto> CreateAsync(WarehouseRequestDto request)
        {
            var entity = _mapper.Map<Warehouse>(request);
            entity.Id = Guid.NewGuid();
            await _repository.AddOneAsync(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<WarehouseResponseDto>(entity);
        }

        public async Task<WarehouseResponseDto?> UpdateAsync(Guid id, WarehouseRequestDto request)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;

            entity.Uid = request.Uid;
            entity.Name = request.Name;
            entity.Status = request.Status;
            entity.ApprovalStatus = request.ApprovalStatus;

            _repository.UpdateOne(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<WarehouseResponseDto>(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return false;
            _repository.DeleteOne(entity);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}