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

        public async Task<ApprovalStepResponseDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            return entity == null ? null : _mapper.Map<ApprovalStepResponseDto>(entity);
        }

        public async Task<ApprovalStepResponseDto> CreateAsync(ApprovalStepRequestDto request)
        {
            var stepOrder = request.StepOrder;
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
            entity.TargetId = request.TargetId;

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
