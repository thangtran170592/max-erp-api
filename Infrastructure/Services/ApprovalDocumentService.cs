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

public class ApprovalDocumentService : IApprovalDocumentService
{
    private readonly IGenericRepository<ApprovalDocument> _repository;
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ApprovalDocumentService(IGenericRepository<ApprovalDocument> repository, IMapper mapper, ApplicationDbContext dbContext)
    {
        _repository = repository;
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ApprovalDocumentResponseDto>> GetAllAsync(Guid? approvalFeatureId = null)
    {
        IQueryable<ApprovalDocument> query = _dbContext.ApprovalDocuments.AsNoTracking();
        if (approvalFeatureId.HasValue) query = query.Where(x => x.ApprovalFeatureId == approvalFeatureId.Value);
        var list = await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        return _mapper.Map<IEnumerable<ApprovalDocumentResponseDto>>(list);
    }

    public async Task<ApiResponseDto<List<ApprovalDocumentResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request, Guid? approvalFeatureId = null, Guid? dataId = null)
    {
        IQueryable<ApprovalDocument> query = _dbContext.ApprovalDocuments.AsNoTracking();
        if (approvalFeatureId.HasValue) query = query.Where(x => x.ApprovalFeatureId == approvalFeatureId.Value);
        if (dataId.HasValue) query = query.Where(x => x.DocumentId == dataId.Value);
        int total = await query.CountAsync();
        if (request.PagedData.Take <= 0) request.PagedData.Take = 10;
        if (request.PagedData.Skip < 0) request.PagedData.Skip = 0;
        var items = await query.OrderByDescending(x => x.CreatedAt)
            .Skip(request.PagedData.Skip)
            .Take(request.PagedData.Take)
            .ToListAsync();
        return new ApiResponseDto<List<ApprovalDocumentResponseDto>>
        {
            Data = _mapper.Map<List<ApprovalDocumentResponseDto>>(items),
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

    public async Task<ApprovalDocumentResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.FindOneAsync(x => x.Id == id);
        return entity == null ? null : _mapper.Map<ApprovalDocumentResponseDto>(entity);
    }

    public async Task<ApprovalDocumentResponseDto> CreateAsync(ApprovalDocumentDto request)
    {
        // Uniqueness: one instance per feature + data id (if that's a rule; implementing here)
        var exist = await _dbContext.ApprovalDocuments.AnyAsync(x => x.ApprovalFeatureId == request.ApprovalFeatureId && x.DocumentId == request.DataId && x.ApprovalStatus != ApprovalStatus.Rejected);
        if (exist) throw new Exception("An active approval instance already exists for this feature and data item");

        var entity = _mapper.Map<ApprovalDocument>(request);
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.CreatedBy = request.CreatedBy;
        if (entity.CurrentStepOrder <= 0) entity.CurrentStepOrder = 1;
        await _repository.AddOneAsync(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<ApprovalDocumentResponseDto>(entity);
    }

    public async Task<ApprovalDocumentResponseDto?> UpdateAsync(Guid id, ApprovalDocumentDto request)
    {
        var entity = await _repository.FindOneAsync(x => x.Id == id);
        if (entity == null) return null;
        // Do not allow changing feature or data id for safety (could allow if needed)
        entity.CurrentStepOrder = request.CurrentStepOrder <= 0 ? entity.CurrentStepOrder : request.CurrentStepOrder;
        // Workaround ambiguity retrieving Status property (compiler issue) via pattern matching
        var requestedStatus = request is { Status: var s } ? s : entity.ApprovalStatus;
        if (entity.ApprovalStatus != requestedStatus)
        {
            entity.ApprovalStatus = requestedStatus;
            // if (entity.Status == ApprovalStatus.Rejected) {}
            //     /entity.ReasonRejection = request.ReasonRejection;

            // else if (entity.Status == ApprovalStatus.Approved)
            //     entity.ReasonRejection = null;
        }
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = request.UpdatedBy;
        _repository.UpdateOne(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<ApprovalDocumentResponseDto>(entity);
    }

    public async Task<ApprovalDocumentResponseDto?> UpdateStatusAsync(Guid id, UpdateApprovalDocumentStatusRequestDto request)
    {
        var entity = await _repository.FindOneAsync(x => x.Id == id);
        if (entity == null) return null;
        // if (entity.Status != request.Status)
        // {
        //     entity.Status = request.Status;
        //     if (request.Status == ApprovalStatus.Rejected)
        //         entity.ReasonRejection = request.ReasonRejection;
        //     else if (request.Status == ApprovalStatus.Approved)
        //         entity.ReasonRejection = null;
        // }
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = request.UpdatedBy;
        _repository.UpdateOne(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<ApprovalDocumentResponseDto>(entity);
    }

    public async Task<ApprovalDocumentResponseDto?> UpdateCurrentStepAsync(Guid id, UpdateApprovalDocumentCurrentStepRequestDto request)
    {
        var entity = await _repository.FindOneAsync(x => x.Id == id);
        if (entity == null) return null;
        if (request.CurrentStepOrder <= 0) throw new Exception("CurrentStepOrder must be greater than zero");
        entity.CurrentStepOrder = request.CurrentStepOrder;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = request.UpdatedBy;
        _repository.UpdateOne(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<ApprovalDocumentResponseDto>(entity);
    }

    public async Task<int> DeleteAsync(Guid id, Guid? deletedBy)
    {
        var entity = await _repository.FindOneAsync(x => x.Id == id);
        if (entity == null) return 0;
        _repository.DeleteOne(entity);
        await _repository.SaveChangesAsync();
        return 1;
    }

    public async Task<bool> IsExistAsync(Expression<Func<ApprovalDocument, bool>> predicate)
        => await _repository.FindOneAsync(predicate) != null;
}
