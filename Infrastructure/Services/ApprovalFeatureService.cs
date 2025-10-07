using System.Linq.Expressions;
using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Core.Entities;
using Infrastructure.Data;

namespace Infrastructure.Services
{
    public class ApprovalFeatureService : IApprovalFeatureService
    {
        private readonly IGenericRepository<ApprovalFeature> _repository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public ApprovalFeatureService(IGenericRepository<ApprovalFeature> repository, IMapper mapper, ApplicationDbContext dbContext)
        {
            _repository = repository;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ApprovalFeatureResponseDto>> GetAllAsync(Guid? approvalConfigId = null)
        {
            var filters = new List<Expression<Func<ApprovalFeature, bool>>>();
            if (approvalConfigId.HasValue)
            {
                filters.Add(x => x.ApprovalConfigId == approvalConfigId.Value);
            }
            var entities = await _repository.FindAllAsync(filters.Count == 0 ? null : filters.ToArray());
            return _mapper.Map<IEnumerable<ApprovalFeatureResponseDto>>(entities);
        }

        public async Task<IEnumerable<ApprovalFeatureResponseDto>> GetManyAsync(Expression<Func<ApprovalFeature, bool>> predicate)
        {
            var entities = await _repository.FindManyAsync(predicate);
            return _mapper.Map<IEnumerable<ApprovalFeatureResponseDto>>(entities);
        }

        public async Task<ApiResponseDto<List<ApprovalFeatureResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request, Guid? approvalConfigId = null)
        {
            Expression<Func<ApprovalFeature, object>>[] includes = [static x => x.ApprovalConfig];
            var result = await _repository.FindManyWithPagingAsync(request, includes);
            return new ApiResponseDto<List<ApprovalFeatureResponseDto>>
            {
                Data = _mapper.Map<List<ApprovalFeatureResponseDto>>(result.Data),
                PageData = result.PageData,
                Message = result.Message,
                Success = result.Success,
                Timestamp = result.Timestamp,
            };
        }

        public async Task<ApprovalFeatureResponseDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            return entity == null ? null : _mapper.Map<ApprovalFeatureResponseDto>(entity);
        }

        public async Task<ApprovalFeatureResponseDto?> GetByUidAsync(Guid approvalConfigId, string uid)
        {
            var entity = await _repository.FindOneAsync(x => x.ApprovalConfigId == approvalConfigId && x.Uid == uid);
            return entity == null ? null : _mapper.Map<ApprovalFeatureResponseDto>(entity);
        }

        public async Task<ApprovalFeatureResponseDto> CreateAsync(ApprovalFeatureRequestDto request)
        {
            // uniqueness: same ApprovalConfig cannot have duplicate Uid
            var existed = await _repository.FindOneAsync(x => x.ApprovalConfigId == request.ApprovalConfigId && x.Uid == request.Uid);
            if (existed != null) throw new Exception($"ApprovalFeature with Uid {request.Uid} already exists in this config");

            var entity = _mapper.Map<ApprovalFeature>(request);
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = request.CreatedBy;
            await _repository.AddOneAsync(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ApprovalFeatureResponseDto>(entity);
        }

        public async Task<ApprovalFeatureResponseDto?> UpdateAsync(Guid id, ApprovalFeatureRequestDto request)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;

            if (!string.Equals(entity.Uid, request.Uid, StringComparison.OrdinalIgnoreCase) || entity.ApprovalConfigId != request.ApprovalConfigId)
            {
                var existed = await _repository.FindOneAsync(x => x.ApprovalConfigId == request.ApprovalConfigId && x.Uid == request.Uid);
                if (existed != null) throw new Exception($"ApprovalFeature with Uid {request.Uid} already exists in this config");
                entity.Uid = request.Uid;
                entity.ApprovalConfigId = request.ApprovalConfigId;
            }
            entity.TargetType = request.TargetType;
            entity.TargetId = request.TargetId;
            entity.Status = request.Status;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = request.UpdatedBy;
            _repository.UpdateOne(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ApprovalFeatureResponseDto>(entity);
        }

        public async Task<ApprovalFeatureResponseDto?> UpdateStatusAsync(Guid id, UpdateApprovalFeatureStatusRequestDto request)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;
            entity.Status = request.Status;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = request.UpdatedBy;
            _repository.UpdateOne(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ApprovalFeatureResponseDto>(entity);
        }

        public async Task<int> DeleteAsync(Guid id, Guid? deletedBy)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return 0;
            _repository.DeleteOne(entity);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> IsExistAsync(Expression<Func<ApprovalFeature, bool>> predicate)
            => await _repository.FindOneAsync(predicate) != null;
    }
}
