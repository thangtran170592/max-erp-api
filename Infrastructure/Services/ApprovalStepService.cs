using System.Linq.Expressions;
using Application.Dtos;
using Application.Common.Models;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class ApprovalStepService : IApprovalStepService
    {
        private readonly IGenericRepository<ApprovalStep> _repository;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ApprovalStepService(IGenericRepository<ApprovalStep> repository, IMapper mapper, ApplicationDbContext dbContext)
        {
            _repository = repository;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ApprovalStepResponseDto>> GetAllAsync(Guid? featureId = null)
        {
            IQueryable<ApprovalStep> query = _dbContext.ApprovalSteps.AsNoTracking();
            if (featureId.HasValue) query = query.Where(x => x.ApprovalFeatureId == featureId.Value);
            query = query.OrderBy(x => x.StepOrder);
            var list = await query.ToListAsync();
            return _mapper.Map<IEnumerable<ApprovalStepResponseDto>>(list);
        }

        public async Task<ApiResponseDto<List<ApprovalStepResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request, Guid? featureId = null)
        {
            // Basic approach: apply featureId filter manually then simple paging (bypassing generic filter complexity for non-string fields)
            IQueryable<ApprovalStep> query = _dbContext.ApprovalSteps.AsNoTracking();
            if (featureId.HasValue) query = query.Where(x => x.ApprovalFeatureId == featureId.Value);
            // TODO: Could extend to dynamic filtering similar to repository if needed.
            var total = await query.CountAsync();
            if (request.PagedData.Take <= 0) request.PagedData.Take = 10;
            if (request.PagedData.Skip < 0) request.PagedData.Skip = 0;
            var items = await query.OrderBy(x => x.StepOrder)
                .Skip(request.PagedData.Skip)
                .Take(request.PagedData.Take)
                .ToListAsync();
            return new ApiResponseDto<List<ApprovalStepResponseDto>>
            {
                Data = _mapper.Map<List<ApprovalStepResponseDto>>(items),
                PageData = new PagedData
                {
                    Skip = request.PagedData.Skip,
                    Take = request.PagedData.Take,
                    TotalCount = total
                },
                Success = true,
                Message = "Data retrieved successfully",
                Timestamp = DateTime.UtcNow
            };
        }

        public async Task<ApprovalStepResponseDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            return entity == null ? null : _mapper.Map<ApprovalStepResponseDto>(entity);
        }

        public async Task<ApprovalStepResponseDto> CreateAsync(ApprovalStepRequestDto request)
        {
            // Determine next StepOrder if not provided or <=0
            var stepsQuery = _dbContext.ApprovalSteps.Where(x => x.ApprovalFeatureId == request.ApprovalFeatureId);
            int nextOrder = await stepsQuery.AnyAsync() ? await stepsQuery.MaxAsync(x => x.StepOrder) + 1 : 1;
            var stepOrder = request.StepOrder.HasValue && request.StepOrder.Value > 0 ? request.StepOrder.Value : nextOrder;

            // If explicit order inserted in middle shift subsequent
            if (stepOrder != nextOrder)
            {
                await _dbContext.Database.BeginTransactionAsync();
                await _dbContext.ApprovalSteps
                    .Where(x => x.ApprovalFeatureId == request.ApprovalFeatureId && x.StepOrder >= stepOrder)
                    .ForEachAsync(x => x.StepOrder += 1);
                await _dbContext.SaveChangesAsync();
            }

            // Enforce single final step
            if (request.IsFinalStep)
            {
                bool existingFinal = await stepsQuery.AnyAsync(x => x.IsFinalStep);
                if (existingFinal) throw new Exception("A final step already exists for this feature");
            }

            var entity = _mapper.Map<ApprovalStep>(request);
            entity.Id = Guid.NewGuid();
            entity.StepOrder = stepOrder;
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = request.CreatedBy;
            await _repository.AddOneAsync(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ApprovalStepResponseDto>(entity);
        }

        public async Task<ApprovalStepResponseDto?> UpdateAsync(Guid id, ApprovalStepRequestDto request)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;

            // Changing StepOrder handled separately (use UpdateOrder endpoint for clarity)
            entity.TargetType = request.TargetType;
            entity.TargetId = request.TargetValue;

            if (request.IsFinalStep != entity.IsFinalStep)
            {
                if (request.IsFinalStep)
                {
                    bool existingFinal = await _dbContext.ApprovalSteps.AnyAsync(x => x.ApprovalFeatureId == entity.ApprovalFeatureId && x.IsFinalStep && x.Id != entity.Id);
                    if (existingFinal) throw new Exception("A final step already exists for this feature");
                }
                entity.IsFinalStep = request.IsFinalStep;
            }
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = request.UpdatedBy;
            _repository.UpdateOne(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ApprovalStepResponseDto>(entity);
        }

        public async Task<ApprovalStepResponseDto?> UpdateOrderAsync(Guid id, UpdateApprovalStepOrderRequestDto request)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;
            var featureId = entity.ApprovalFeatureId;
            if (request.StepOrder <= 0) throw new Exception("StepOrder must be greater than zero");

            if (request.StepOrder == entity.StepOrder)
            {
                return _mapper.Map<ApprovalStepResponseDto>(entity); // no change
            }

            await using var tx = await _dbContext.Database.BeginTransactionAsync();
            var steps = await _dbContext.ApprovalSteps.Where(x => x.ApprovalFeatureId == featureId).ToListAsync();
            int original = entity.StepOrder;
            int target = request.StepOrder;

            if (target > steps.Count) target = steps.Count; // clamp to end

            // Adjust others
            if (target < original)
            {
                foreach (var s in steps.Where(s => s.StepOrder >= target && s.StepOrder < original))
                    s.StepOrder += 1;
            }
            else
            {
                foreach (var s in steps.Where(s => s.StepOrder <= target && s.StepOrder > original))
                    s.StepOrder -= 1;
            }
            entity.StepOrder = target;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = request.UpdatedBy;
            await _dbContext.SaveChangesAsync();
            await tx.CommitAsync();
            return _mapper.Map<ApprovalStepResponseDto>(entity);
        }

        public async Task<ApprovalStepResponseDto?> UpdateIsFinalAsync(Guid id, UpdateApprovalStepStatusRequestDto request)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;
            if (request.IsFinalStep && !entity.IsFinalStep)
            {
                bool existingFinal = await _dbContext.ApprovalSteps.AnyAsync(x => x.ApprovalFeatureId == entity.ApprovalFeatureId && x.IsFinalStep && x.Id != entity.Id);
                if (existingFinal) throw new Exception("A final step already exists for this feature");
            }
            entity.IsFinalStep = request.IsFinalStep;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = request.UpdatedBy;
            _repository.UpdateOne(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ApprovalStepResponseDto>(entity);
        }

        public async Task<int> DeleteAsync(Guid id, Guid? deletedBy)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return 0;
            var featureId = entity.ApprovalFeatureId;
            int order = entity.StepOrder;
            _repository.DeleteOne(entity);
            await _repository.SaveChangesAsync();
            // Rebalance following steps
            var following = await _dbContext.ApprovalSteps.Where(x => x.ApprovalFeatureId == featureId && x.StepOrder > order).ToListAsync();
            foreach (var f in following) f.StepOrder -= 1;
            await _dbContext.SaveChangesAsync();
            return 1;
        }

        public async Task<bool> IsExistAsync(Expression<Func<ApprovalStep, bool>> predicate)
            => await _repository.FindOneAsync(predicate) != null;

        public async Task RebalanceOrdersAsync(Guid approvalFeatureId)
        {
            var steps = await _dbContext.ApprovalSteps.Where(x => x.ApprovalFeatureId == approvalFeatureId).OrderBy(x => x.StepOrder).ToListAsync();
            int i = 1;
            foreach (var s in steps)
            {
                if (s.StepOrder != i)
                {
                    s.StepOrder = i;
                }
                i++;
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
