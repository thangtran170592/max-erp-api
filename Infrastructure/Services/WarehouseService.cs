using Application.Common.Helpers;
using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IGenericRepository<Warehouse> _wareHouseRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly IGenericRepository<WarehouseHistory> _wareHouseHistoryRepository;
        private readonly IMapper _mapper;

        public WarehouseService(IGenericRepository<Warehouse> wareHouseRepository, IGenericRepository<WarehouseHistory> wareHouseHistoryRepository, IMapper mapper, ApplicationDbContext dbContext)
        {
            _wareHouseRepository = wareHouseRepository;
            _wareHouseHistoryRepository = wareHouseHistoryRepository;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<WarehouseResponseDto>> GetAllAsync()
        {
            var warehouses = await _wareHouseRepository.FindAllAsync();
            return _mapper.Map<IEnumerable<WarehouseResponseDto>>(warehouses);
        }

        public async Task<IEnumerable<WarehouseResponseDto>> GetManyAsync(System.Linq.Expressions.Expression<Func<Warehouse, bool>> predicate)
        {
            var entities = await _wareHouseRepository.FindManyAsync(predicate);
            return _mapper.Map<IEnumerable<WarehouseResponseDto>>(entities);
        }

        public async Task<ApiResponseDto<List<WarehouseResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request)
        {
            var result = await _wareHouseRepository.FindManyWithPagingAsync(request);
            return new ApiResponseDto<List<WarehouseResponseDto>>
            {
                Data = _mapper.Map<List<WarehouseResponseDto>>(result.Data),
                PageData = result.PageData,
                Message = result.Message,
                Success = result.Success,
                Timestamp = result.Timestamp,
            };
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
            warehouseHistory.Id = Guid.NewGuid();
            await _wareHouseHistoryRepository.AddOneAsync(warehouseHistory);

            await _wareHouseRepository.SaveChangesAsync();
            return _mapper.Map<WarehouseResponseDto>(entity);
        }

        public async Task<bool> IsExistAsync(System.Linq.Expressions.Expression<Func<Warehouse, bool>> predicate)
        {
            return await _wareHouseRepository.FindOneAsync(predicate) != null;
        }

        public async Task<WarehouseResponseDto?> UpdateAsync(Guid id, WarehouseRequestDto request)
        {
            var wareHouse = await _wareHouseRepository.FindOneAsync(x => x.Id == id);
            if (wareHouse == null) return null;

            wareHouse.Name = request.Name;
            wareHouse.Status = request.Status;
            wareHouse.UpdatedAt = DateTime.UtcNow;
            wareHouse.UpdatedBy = request.UpdatedBy;

            _wareHouseRepository.UpdateOne(wareHouse);

            var warehouseHistory = _mapper.Map<WarehouseHistory>(wareHouse);
            warehouseHistory.Id = Guid.NewGuid();
            warehouseHistory.Uid = wareHouse.Uid;
            warehouseHistory.WarehouseId = wareHouse.Id;
            warehouseHistory.Status = request.Status;
            warehouseHistory.UpdatedAt = DateTime.UtcNow;
            warehouseHistory.UpdatedBy = request.UpdatedBy;
            await _wareHouseHistoryRepository.AddOneAsync(warehouseHistory);

            await _wareHouseRepository.SaveChangesAsync();
            return _mapper.Map<WarehouseResponseDto>(wareHouse);
        }

        public async Task<int> DeleteAsync(Guid id, Guid deletedBy)
        {
            var entity = await _wareHouseRepository.FindOneAsync(x => x.Id == id);
            if (entity == null) return 0;
            _wareHouseRepository.DeleteOne(entity);
            var result = await _wareHouseRepository.SaveChangesAsync();
            return result;
        }

        public async Task<WarehouseResponseDto> UpdateStatusAsync(Guid id, UpdateWarehouseStatusRequestDto request)
        {
            var wareHouse = await _wareHouseRepository.FindOneAsync(x => x.Id == id);
            if (wareHouse == null) throw new Exception("Warehouse not found");

            wareHouse.ApprovalStatus = request.ApprovalStatus;
            wareHouse.UpdatedAt = DateTime.UtcNow;
            wareHouse.UpdatedBy = request.UpdatedBy;

            _wareHouseRepository.UpdateOne(wareHouse);

            var warehouseHistory = _mapper.Map<WarehouseHistory>(wareHouse);
            warehouseHistory.Id = Guid.NewGuid();
            warehouseHistory.Uid = wareHouse.Uid;
            warehouseHistory.WarehouseId = wareHouse.Id;
            warehouseHistory.ReasonRejection = request.ReasonRejection;
            warehouseHistory.ApprovalStatus = request.ApprovalStatus;
            warehouseHistory.UpdatedAt = DateTime.UtcNow;
            warehouseHistory.UpdatedBy = request.UpdatedBy;

            await _wareHouseHistoryRepository.AddOneAsync(warehouseHistory);

            await _wareHouseRepository.SaveChangesAsync();
            return _mapper.Map<WarehouseResponseDto>(wareHouse);
        }

        public async Task<IEnumerable<WarehouseHistoryDto>> GetWarehouseHistoryAsync(Guid warehouseId)
        {
            var warehouseHistoriesQuery = from warehouseHistory in _dbContext.WarehouseHistories
                                          where warehouseHistory.WarehouseId == warehouseId
                                          join creatorUser in _dbContext.Users on warehouseHistory.CreatedBy equals creatorUser.Id into createdGroup
                                          from createdUser in createdGroup.DefaultIfEmpty()
                                          join updatedUser in _dbContext.Users on warehouseHistory.UpdatedBy equals updatedUser.Id into updatedGroup
                                          from updatedUser in updatedGroup.DefaultIfEmpty()
                                          select new WarehouseHistoryDto
                                          {
                                              WarehouseId = warehouseHistory.WarehouseId,
                                              Uid = warehouseHistory.Uid,
                                              Name = warehouseHistory.Name,
                                              ReasonRejection = warehouseHistory.ReasonRejection,
                                              Status = warehouseHistory.Status,
                                              ApprovalStatus = warehouseHistory.ApprovalStatus,
                                              CreatedAt = warehouseHistory.CreatedAt,
                                              CreatedBy = warehouseHistory.CreatedBy,
                                              UpdatedAt = warehouseHistory.UpdatedAt,
                                              UpdatedBy = warehouseHistory.UpdatedBy,
                                              CreatedByUser = createdUser == null ? null : new UserResponseDto
                                              {
                                                  Id = createdUser.Id,
                                                  FullName = createdUser.FullName,
                                                  UserName = createdUser.UserName,
                                                  Email = createdUser.Email,
                                                  ProfilePicture = createdUser.ProfilePicture,
                                              },
                                              UpdatedByUser = updatedUser == null ? null : new UserResponseDto
                                              {
                                                  Id = updatedUser.Id,
                                                  FullName = updatedUser.FullName,
                                                  UserName = updatedUser.UserName,
                                                  Email = updatedUser.Email,
                                                  ProfilePicture = updatedUser.ProfilePicture,
                                              }
                                          };
            var histories = await warehouseHistoriesQuery.AsNoTracking().ToListAsync();
            if (histories == null) throw new Exception("Warehouse not found");
            return histories;
        }
    }
}