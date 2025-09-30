using Application.Common.Helpers;
using Application.Common.Models;
using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Core.Entities;
using Core.Enums;

namespace Infrastructure.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IGenericRepository<Warehouse> _wareHouseRepository;
        private readonly IGenericRepository<WarehouseHistory> _wareHouseHistoryRepository;
        private readonly IMapper _mapper;

        public WarehouseService(IGenericRepository<Warehouse> wareHouseRepository, IGenericRepository<WarehouseHistory> wareHouseHistoryRepository, IMapper mapper)
        {
            _wareHouseRepository = wareHouseRepository;
            _wareHouseHistoryRepository = wareHouseHistoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WarehouseResponseDto>> GetAllAsync()
        {
            var warehouses = await _wareHouseRepository.FindAllAsync();
            return _mapper.Map<IEnumerable<WarehouseResponseDto>>(warehouses);
        }

        public async Task<ApiResponseDto<List<WarehouseResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request)
        {
            var pagedResult = await _wareHouseRepository.FindManyWithPagingAsync(request);
            var result = ApiResponseHelper.CreateSuccessResponse<Warehouse, WarehouseResponseDto>(pagedResult, _mapper);
            return result;
        }

        public async Task<WarehouseResponseDto?> GetByIdAsync(Guid id)
        {
            var warehouse = await _wareHouseRepository.FindOneAsync(x => x.Id == id);
            return warehouse == null ? null : _mapper.Map<WarehouseResponseDto>(warehouse);
        }

        public async Task<WarehouseResponseDto> CreateAsync(WarehouseRequestDto request)
        {
            var entity = _mapper.Map<Warehouse>(request);
            entity.Id = Guid.NewGuid();
            entity.Uid = StringHelper.FriendlyUrl(request.Uid!);
            await _wareHouseRepository.AddOneAsync(entity);

            var warehouseHistory = _mapper.Map<WarehouseHistory>(entity);
            warehouseHistory.WarehouseId = entity.Id;
            await _wareHouseHistoryRepository.AddOneAsync(warehouseHistory);

            await _wareHouseRepository.SaveChangesAsync();
            return _mapper.Map<WarehouseResponseDto>(entity);
        }

        public async Task<WarehouseResponseDto?> UpdateAsync(Guid id, WarehouseRequestDto request)
        {
            var entity = await _wareHouseRepository.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;

            entity = _mapper.Map<Warehouse>(request);
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = request.UpdatedBy;

            _wareHouseRepository.UpdateOne(entity);

            var warehouseHistory = _mapper.Map<WarehouseHistory>(entity);
            warehouseHistory.WarehouseId = entity.Id;
            _wareHouseHistoryRepository.UpdateOne(warehouseHistory);

            await _wareHouseRepository.SaveChangesAsync();
            return _mapper.Map<WarehouseResponseDto>(entity);
        }

        public async Task<int> DeleteAsync(Guid id, Guid deletedBy)
        {
            var entity = await _wareHouseRepository.FindOneAsync(x => x.Id == id);
            if (entity == null) return 0;
            _wareHouseRepository.DeleteOne(entity);
            var result = await _wareHouseRepository.SaveChangesAsync();
            return result;
        }

        public async Task<WarehouseResponseDto> UpdateStatusAsync(Guid id, WarehouseStatusUpdateDto request)
        {
            var entity = await _wareHouseRepository.FindOneAsync(x => x.Id == id);
            if (entity == null) throw new Exception("Warehouse not found");

            entity.ApprovalStatus = request.ApprovalStatus;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = request.UpdatedBy;

            _wareHouseRepository.UpdateOne(entity);

            var warehouseHistory = _mapper.Map<WarehouseHistory>(entity);
            warehouseHistory.WarehouseId = entity.Id;
            _wareHouseHistoryRepository.UpdateOne(warehouseHistory);

            await _wareHouseRepository.SaveChangesAsync();
            return _mapper.Map<WarehouseResponseDto>(entity);
        }
    }
}