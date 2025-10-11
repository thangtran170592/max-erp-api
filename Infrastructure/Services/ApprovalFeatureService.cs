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
        private readonly IGenericRepository<ApprovalFeature> _approvalFeatureRepository;
        private readonly IGenericRepository<ApprovalConfig> _approvalConfigRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public ApprovalFeatureService(IGenericRepository<ApprovalFeature> approvalFeatureRepository, IGenericRepository<ApprovalConfig> approvalConfigRepository, IMapper mapper, ApplicationDbContext dbContext)
        {
            _approvalFeatureRepository = approvalFeatureRepository;
            _approvalConfigRepository = approvalConfigRepository;
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
            var entities = await _approvalFeatureRepository.FindAllAsync(filters.Count == 0 ? null : filters.ToArray());
            return _mapper.Map<IEnumerable<ApprovalFeatureResponseDto>>(entities);
        }

        public async Task<IEnumerable<ApprovalFeatureResponseDto>> GetManyAsync(Expression<Func<ApprovalFeature, bool>> predicate)
        {
            var entities = await _approvalFeatureRepository.FindManyAsync(predicate);
            return _mapper.Map<IEnumerable<ApprovalFeatureResponseDto>>(entities);
        }

        public async Task<ApiResponseDto<List<ApprovalFeatureResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request, Guid? approvalConfigId = null)
        {
            Expression<Func<ApprovalFeature, object>>[] includes = [static x => x.ApprovalConfig, x => x.ApprovalSteps];
            var result = await _approvalFeatureRepository.FindManyWithPagingAsync(request, includes);
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
            var entity = await _approvalFeatureRepository.FindOneAsync(x => x.Id == id, [static x => x.ApprovalConfig]);
            return entity == null ? null : _mapper.Map<ApprovalFeatureResponseDto>(entity);
        }

        public async Task<ApprovalFeatureResponseDto> CreateAsync(ApprovalFeatureRequestDto request)
        {
            var existed = await _approvalFeatureRepository.FindOneAsync(x => x.ApprovalConfigId == request.ApprovalConfigId && x.Uid == request.Uid);
            if (existed != null) throw new Exception($"ApprovalFeature with Uid {request.Uid} already exists in this config");

            var feature = _mapper.Map<ApprovalFeature>(request);
            feature.Id = Guid.NewGuid();
            feature.CreatedAt = DateTime.UtcNow;
            feature.CreatedBy = request.CreatedBy;
            
            await _approvalFeatureRepository.AddOneAsync(feature);
            var approvalConfig = await _approvalConfigRepository.FindOneAsync(x => x.Id == request.ApprovalConfigId);

            if (approvalConfig == null) throw new Exception($"ApprovalConfig with Id {request.ApprovalConfigId} not found");
            approvalConfig.LatestApprovalFeatureId = feature.Id;

            _approvalConfigRepository.UpdateOne(approvalConfig);
            if (request.ApprovalSteps != null && request.ApprovalSteps.Any())
            {
                int stepsCount = request.ApprovalSteps.Count;
                var steps = request.ApprovalSteps.Select((step, index) => new ApprovalStep
                {
                    Id = Guid.NewGuid(),
                    ApprovalFeature = feature,
                    StepOrder = step.StepOrder,
                    TargetId = step.TargetId,
                    TargetType = step.TargetType,
                    IsFinalStep = index == stepsCount - 1,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy
                }).ToList();
                await _dbContext.ApprovalSteps.AddRangeAsync(steps);
            }

            await _approvalFeatureRepository.SaveChangesAsync();

            await _dbContext.Entry(feature).Reference(x => x.ApprovalConfig).LoadAsync();
            return _mapper.Map<ApprovalFeatureResponseDto>(feature);
        }

        public async Task<ApprovalFeatureResponseDto?> UpdateAsync(Guid id, ApprovalFeatureRequestDto request)
        {
            var entity = await _approvalFeatureRepository.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;
            entity.Name = request.Name;
            entity.ApprovalConfigId = request.ApprovalConfigId;
            entity.Status = request.Status;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = request.UpdatedBy;
            _approvalFeatureRepository.UpdateOne(entity);
            if (request.ApprovalSteps != null && request.ApprovalSteps.Any())
            {
                var existingSteps = _dbContext.ApprovalSteps.Where(s => s.ApprovalFeatureId == entity.Id);
                _dbContext.ApprovalSteps.RemoveRange(existingSteps);

                int stepsCount = request.ApprovalSteps.Count;
                var steps = request.ApprovalSteps.Select((step, index) => new ApprovalStep
                {
                    Id = Guid.NewGuid(),
                    ApprovalFeatureId = entity.Id,
                    StepOrder = step.StepOrder,
                    TargetId = step.TargetId,
                    TargetType = step.TargetType,
                    IsFinalStep = index == stepsCount - 1,
                    CreatedAt = entity.CreatedAt,
                    CreatedBy = entity.CreatedBy,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = request.UpdatedBy,
                }).ToList();
                await _dbContext.ApprovalSteps.AddRangeAsync(steps);
            }
            await _approvalFeatureRepository.SaveChangesAsync();

            // Load ApprovalConfig before returning response
            await _dbContext.Entry(entity).Reference(x => x.ApprovalConfig).LoadAsync();

            return _mapper.Map<ApprovalFeatureResponseDto>(entity);
        }

        public async Task<int> DeleteAsync(Guid id, Guid? deletedBy)
        {
            var entity = await _approvalFeatureRepository.FindOneAsync(x => x.Id == id);
            if (entity == null) return 0;
            _approvalFeatureRepository.DeleteOne(entity);
            return await _approvalFeatureRepository.SaveChangesAsync();
        }

        public async Task<bool> IsExistAsync(Expression<Func<ApprovalFeature, bool>> predicate)
            => await _approvalFeatureRepository.FindOneAsync(predicate) != null;
    }
}
