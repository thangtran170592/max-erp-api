using Application.Common.Models;
using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Core.Entities;
using Core.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Services;

public class ApprovalHistoryService : IApprovalHistoryService
{
    private readonly IGenericRepository<ApprovalHistory> _repository;
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ApprovalHistoryService(IGenericRepository<ApprovalHistory> repository, IMapper mapper, ApplicationDbContext dbContext)
    {
        _repository = repository;
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ApprovalHistoryResponseDto>> GetAllAsync(Guid? approvalInstanceId = null)
    {
        IQueryable<ApprovalHistory> query = _dbContext.ApprovalHistories.AsNoTracking();
        if (approvalInstanceId.HasValue) query = query.Where(x => x.ApprovalDocumentId == approvalInstanceId.Value);
        var list = await query.OrderBy(x => x.StepOrder).ToListAsync();
        return _mapper.Map<IEnumerable<ApprovalHistoryResponseDto>>(list);
    }

    public async Task<ApiResponseDto<List<ApprovalHistoryResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request, Guid? approvalInstanceId = null)
    {
        IQueryable<ApprovalHistory> query = _dbContext.ApprovalHistories.AsNoTracking();
        if (approvalInstanceId.HasValue) query = query.Where(x => x.ApprovalDocumentId == approvalInstanceId.Value);
        int total = await query.CountAsync();
        if (request.PagedData.Take <= 0) request.PagedData.Take = 10;
        if (request.PagedData.Skip < 0) request.PagedData.Skip = 0;
        var items = await query.OrderBy(x => x.StepOrder)
            .Skip(request.PagedData.Skip)
            .Take(request.PagedData.Take)
            .ToListAsync();
        return new ApiResponseDto<List<ApprovalHistoryResponseDto>>
        {
            Data = _mapper.Map<List<ApprovalHistoryResponseDto>>(items),
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

    public async Task<ApprovalHistoryResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.FindOneAsync(x => x.Id == id);
        return entity == null ? null : _mapper.Map<ApprovalHistoryResponseDto>(entity);
    }

    public async Task<ApprovalHistoryResponseDto> CreateAsync(ApprovalHistoryRequestDto request)
    {
        var actionsQuery = _dbContext.ApprovalHistories.Where(x => x.ApprovalDocumentId == request.ApprovalInstanceId);
        int nextOrder = await actionsQuery.AnyAsync() ? await actionsQuery.MaxAsync(x => x.StepOrder) + 1 : 1;
        int stepOrder = request.StepOrder.HasValue && request.StepOrder.Value > 0 ? request.StepOrder.Value : nextOrder;

        if (stepOrder != nextOrder)
        {
            await using var tx = await _dbContext.Database.BeginTransactionAsync();
            await _dbContext.ApprovalHistories.Where(x => x.ApprovalDocumentId == request.ApprovalInstanceId && x.StepOrder >= stepOrder)
                .ForEachAsync(x => x.StepOrder += 1);
            await _dbContext.SaveChangesAsync();
            await tx.CommitAsync();
        }

        var entity = _mapper.Map<ApprovalHistory>(request);
        entity.Id = Guid.NewGuid();
        entity.StepOrder = stepOrder;
        entity.CreatedAt = DateTime.UtcNow;
        entity.CreatedBy = request.CreatedBy;
        if (entity.Status == ApprovalStatus.Approved)
        {
            entity.ApprovedAt = DateTime.UtcNow;
        }
        await _repository.AddOneAsync(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<ApprovalHistoryResponseDto>(entity);
    }

    public async Task<ApprovalHistoryResponseDto?> UpdateAsync(Guid id, ApprovalHistoryRequestDto request)
    {
        var entity = await _repository.FindOneAsync(x => x.Id == id);
        if (entity == null) return null;
        // StepOrder changes handled in UpdateOrder
        entity.ApproverId = request.ApproverId;
        if (entity.Status != request.Status)
        {
            entity.Status = request.Status;
            entity.ApprovedAt = request.Status == ApprovalStatus.Approved ? DateTime.UtcNow : null;
        }
        entity.Reason = request.Reason;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = request.UpdatedBy;
        _repository.UpdateOne(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<ApprovalHistoryResponseDto>(entity);
    }

    public async Task<ApprovalHistoryResponseDto?> UpdateStatusAsync(Guid id, UpdateApprovalActionStatusRequestDto request)
    {
        var entity = await _repository.FindOneAsync(x => x.Id == id);
        if (entity == null) return null;
        if (entity.Status != request.Status)
        {
            entity.Status = request.Status;
            entity.ApprovedAt = request.Status == ApprovalStatus.Approved ? DateTime.UtcNow : null;
        }
        entity.Reason = request.Reason;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = request.UpdatedBy;
        _repository.UpdateOne(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<ApprovalHistoryResponseDto>(entity);
    }

    public async Task<ApprovalHistoryResponseDto?> UpdateOrderAsync(Guid id, UpdateApprovalActionOrderRequestDto request)
    {
        var entity = await _repository.FindOneAsync(x => x.Id == id);
        if (entity == null) return null;
        if (request.StepOrder <= 0) throw new Exception("StepOrder must be greater than zero");
        if (request.StepOrder == entity.StepOrder) return _mapper.Map<ApprovalHistoryResponseDto>(entity);

        var instanceId = entity.ApprovalDocumentId;
        await using var tx = await _dbContext.Database.BeginTransactionAsync();
        var actions = await _dbContext.ApprovalHistories.Where(x => x.ApprovalDocumentId == instanceId).ToListAsync();
        int original = entity.StepOrder;
        int target = request.StepOrder;
        if (target > actions.Count) target = actions.Count;

        if (target < original)
        {
            foreach (var a in actions.Where(a => a.StepOrder >= target && a.StepOrder < original))
                a.StepOrder += 1;
        }
        else
        {
            foreach (var a in actions.Where(a => a.StepOrder <= target && a.StepOrder > original))
                a.StepOrder -= 1;
        }
        entity.StepOrder = target;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = request.UpdatedBy;
        await _dbContext.SaveChangesAsync();
        await tx.CommitAsync();
        return _mapper.Map<ApprovalHistoryResponseDto>(entity);
    }

    public async Task<int> DeleteAsync(Guid id, Guid? deletedBy)
    {
        var entity = await _repository.FindOneAsync(x => x.Id == id);
        if (entity == null) return 0;
        var instanceId = entity.ApprovalDocumentId;
        int order = entity.StepOrder;
        _repository.DeleteOne(entity);
        await _repository.SaveChangesAsync();
        var following = await _dbContext.ApprovalHistories.Where(x => x.ApprovalDocumentId == instanceId && x.StepOrder > order).ToListAsync();
        foreach (var f in following) f.StepOrder -= 1;
        await _dbContext.SaveChangesAsync();
        return 1;
    }

    public async Task<bool> IsExistAsync(Expression<Func<ApprovalHistory, bool>> predicate)
        => await _repository.FindOneAsync(predicate) != null;

    public async Task RebalanceOrdersAsync(Guid approvalInstanceId)
    {
        var actions = await _dbContext.ApprovalHistories.Where(x => x.ApprovalDocumentId == approvalInstanceId).OrderBy(x => x.StepOrder).ToListAsync();
        int i = 1;
        foreach (var a in actions)
        {
            if (a.StepOrder != i) a.StepOrder = i;
            i++;
        }
        await _dbContext.SaveChangesAsync();
    }
}
