using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Core.Entities;

namespace Infrastructure.Services
{
    public class WarehouseHistoryService : IWarehouseHistoryService
    {
        private readonly IGenericRepository<WarehouseHistory> _repository;
        private readonly IMapper _mapper;

        public WarehouseHistoryService(IGenericRepository<WarehouseHistory> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WarehouseHistoryDto>> GetAllAsync()
        {
            var histories = await _repository.FindAllAsync();
            return _mapper.Map<IEnumerable<WarehouseHistoryDto>>(histories);
        }

        public async Task<WarehouseHistoryDto?> GetByIdAsync(Guid id)
        {
            var history = await _repository.FindOneAsync(x => x.Id == id);
            return history == null ? null : _mapper.Map<WarehouseHistoryDto>(history);
        }

        public async Task<WarehouseHistoryDto> CreateAsync(WarehouseHistoryDto dto)
        {
            var entity = _mapper.Map<WarehouseHistory>(dto);
            entity.Id = Guid.NewGuid();
            await _repository.AddOneAsync(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<WarehouseHistoryDto>(entity);
        }

        public async Task<WarehouseHistoryDto?> UpdateAsync(Guid id, WarehouseHistoryDto dto)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;

            entity.WarehouseId = dto.WarehouseId;
            entity.Uid = dto.Uid;
            entity.Name = dto.Name;
            entity.Status = dto.Status;
            entity.ApprovalStatus = dto.ApprovalStatus;
            entity.ChangedAt = dto.ChangedAt;
            entity.ChangedBy = dto.ChangedBy;

            _repository.UpdateOne(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<WarehouseHistoryDto>(entity);
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