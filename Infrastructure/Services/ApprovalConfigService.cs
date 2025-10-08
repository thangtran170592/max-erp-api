using System.Linq.Expressions;
using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Core.Entities;

namespace Infrastructure.Services
{
    public class ApprovalConfigService : IApprovalConfigService
    {
        private readonly IGenericRepository<ApprovalConfig> _repository;
        private readonly IMapper _mapper;

        public ApprovalConfigService(IGenericRepository<ApprovalConfig> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ApprovalConfigResponseDto>> GetAllAsync()
        {
            var entities = await _repository.FindAllAsync();
            return _mapper.Map<IEnumerable<ApprovalConfigResponseDto>>(entities);
        }

        public async Task<IEnumerable<ApprovalConfigResponseDto>> GetManyAsync(Expression<Func<ApprovalConfig, bool>> predicate)
        {
            var entities = await _repository.FindManyAsync(predicate);
            return _mapper.Map<IEnumerable<ApprovalConfigResponseDto>>(entities);
        }

        public async Task<ApiResponseDto<List<ApprovalConfigResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request)
        {
            var result = await _repository.FindManyWithPagingAsync(request);
            return new ApiResponseDto<List<ApprovalConfigResponseDto>>
            {
                Data = _mapper.Map<List<ApprovalConfigResponseDto>>(result.Data),
                PageData = result.PageData,
                Message = result.Message,
                Success = result.Success,
                Timestamp = result.Timestamp,
            };
        }

        public async Task<ApprovalConfigResponseDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            return entity == null ? null : _mapper.Map<ApprovalConfigResponseDto>(entity);
        }

        public async Task<ApprovalConfigResponseDto?> GetByUidAsync(string uid)
        {
            var entity = await _repository.FindOneAsync(x => x.Uid == uid);
            return entity == null ? null : _mapper.Map<ApprovalConfigResponseDto>(entity);
        }

        public async Task<ApprovalConfigResponseDto> CreateAsync(ApprovalConfigRequestDto request)
        {
            // uniqueness
            var existed = await _repository.FindOneAsync(x => x.Uid == request.Uid);
            if (existed != null) throw new Exception($"ApprovalConfig with Uid {request.Uid} already exists");

            var entity = _mapper.Map<ApprovalConfig>(request);
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = request.CreatedBy;
            await _repository.AddOneAsync(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ApprovalConfigResponseDto>(entity);
        }

        public async Task<ApprovalConfigResponseDto?> UpdateAsync(Guid id, ApprovalConfigRequestDto request)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;

            if (!string.Equals(entity.Uid, request.Uid, StringComparison.OrdinalIgnoreCase))
            {
                var existed = await _repository.FindOneAsync(x => x.Uid == request.Uid);
                if (existed != null) throw new Exception($"ApprovalConfig with Uid {request.Uid} already exists");
                entity.Uid = request.Uid;
            }
            entity.Name = request.Name;
            entity.Description = request.Description ?? string.Empty;
            entity.Status = request.Status;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = request.UpdatedBy;
            _repository.UpdateOne(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ApprovalConfigResponseDto>(entity);
        }

        public async Task<ApprovalConfigResponseDto?> UpdateStatusAsync(Guid id, UpdateApprovalConfigStatusRequestDto request)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;
            entity.Status = request.Status;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = request.UpdatedBy;
            _repository.UpdateOne(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ApprovalConfigResponseDto>(entity);
        }

        public async Task<int> DeleteAsync(Guid id, Guid? deletedBy)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return 0;
            _repository.DeleteOne(entity);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> IsExistAsync(Expression<Func<ApprovalConfig, bool>> predicate)
            => await _repository.FindOneAsync(predicate) != null;
    }
}
