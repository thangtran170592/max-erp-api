using Application.Common.Helpers;
using Application.Common.Models;
using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Core.Entities;
using Core.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IGenericRepository<Warehouse> _wareHouseRepository;
        private readonly IGenericRepository<ApprovalHistory> _approvalHistoryRepository;
        private readonly IGenericRepository<ApprovalDocument> _approvalDocumentRepository;
        private readonly IMapper _mapper;

        public WarehouseService(IGenericRepository<Warehouse> wareHouseRepository, IGenericRepository<ApprovalHistory> approvalHistoryRepository, IGenericRepository<ApprovalDocument> approvalDocumentRepository, IMapper mapper, ApplicationDbContext dbContext)
        {
            _wareHouseRepository = wareHouseRepository;
            _approvalHistoryRepository = approvalHistoryRepository;
            _approvalDocumentRepository = approvalDocumentRepository;
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

        public async Task<ApiResponseDto<List<WarehouseResponseDto>>> GetManyWithPagingAsync(Guid userId, Guid? departmentId, Guid? positionId, FilterRequestDto request)
        {
            // Get warehouses with their editable status where the current user is either:
            // 1. An approver in the approval history, OR
            // 2. The creator of the approval document
            // Using JOINs instead of nested queries for better performance
            var warehousesWithEditableQuery = (from war in _dbContext.Warehouses
                                               join doc in _dbContext.ApprovalDocuments on war.Id equals doc.DocumentId
                                               join his in _dbContext.ApprovalHistories on doc.Id equals his.ApprovalDocumentId into hisGroup
                                               from his in hisGroup.DefaultIfEmpty()
                                               where doc.CreatedBy == userId || his.ApproverId == userId || his.ApproverId == departmentId || his.ApproverId == positionId
                                               select new { Warehouse = war, Document = doc })
                                              .Distinct();

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                warehousesWithEditableQuery = warehousesWithEditableQuery.Where(w =>
                    w.Warehouse.Name.Contains(request.SearchTerm) ||
                    w.Warehouse.Uid.Contains(request.SearchTerm));
            }

            // Apply dynamic filters from FilterRequestDto
            if (request.Filters != null && request.Filters.Any())
            {
                foreach (var filter in request.Filters)
                {
                    switch (filter.Field.ToLower())
                    {
                        case "name":
                            if (!string.IsNullOrEmpty(filter.Value))
                                warehousesWithEditableQuery = warehousesWithEditableQuery.Where(w => w.Warehouse.Name.Contains(filter.Value));
                            break;
                        case "uid":
                            if (!string.IsNullOrEmpty(filter.Value))
                                warehousesWithEditableQuery = warehousesWithEditableQuery.Where(w => w.Warehouse.Uid.Contains(filter.Value));
                            break;
                        case "status":
                            if (bool.TryParse(filter.Value, out bool statusValue))
                                warehousesWithEditableQuery = warehousesWithEditableQuery.Where(w => w.Warehouse.Status == statusValue);
                            break;
                        case "approvalstatus":
                            if (Enum.TryParse<ApprovalStatus>(filter.Value, true, out ApprovalStatus approvalStatus))
                                warehousesWithEditableQuery = warehousesWithEditableQuery.Where(w => w.Warehouse.ApprovalStatus == approvalStatus);
                            break;
                        case "createdby":
                            if (Guid.TryParse(filter.Value, out Guid createdBy))
                                warehousesWithEditableQuery = warehousesWithEditableQuery.Where(w => w.Warehouse.CreatedBy == createdBy);
                            break;
                        case "createdat":
                            if (DateTime.TryParse(filter.Value, out DateTime createdAt))
                                warehousesWithEditableQuery = warehousesWithEditableQuery.Where(w => w.Warehouse.CreatedAt.Date == createdAt.Date);
                            break;
                        case "editable":
                            if (bool.TryParse(filter.Value, out bool editableValue))
                                warehousesWithEditableQuery = warehousesWithEditableQuery.Where(w => w.Document.Editable == editableValue);
                            break;
                    }
                }
            }

            // Apply sorting (simplified - just use first sort for now)
            if (request.Sorts != null && request.Sorts.Any())
            {
                var firstSort = request.Sorts.First();
                switch (firstSort.Field.ToLower())
                {
                    case "name":
                        warehousesWithEditableQuery = firstSort.Dir.ToLower() == "desc" 
                            ? warehousesWithEditableQuery.OrderByDescending(w => w.Warehouse.Name)
                            : warehousesWithEditableQuery.OrderBy(w => w.Warehouse.Name);
                        break;
                    case "uid":
                        warehousesWithEditableQuery = firstSort.Dir.ToLower() == "desc" 
                            ? warehousesWithEditableQuery.OrderByDescending(w => w.Warehouse.Uid)
                            : warehousesWithEditableQuery.OrderBy(w => w.Warehouse.Uid);
                        break;
                    case "status":
                        warehousesWithEditableQuery = firstSort.Dir.ToLower() == "desc" 
                            ? warehousesWithEditableQuery.OrderByDescending(w => w.Warehouse.Status)
                            : warehousesWithEditableQuery.OrderBy(w => w.Warehouse.Status);
                        break;
                    case "approvalstatus":
                        warehousesWithEditableQuery = firstSort.Dir.ToLower() == "desc" 
                            ? warehousesWithEditableQuery.OrderByDescending(w => w.Warehouse.ApprovalStatus)
                            : warehousesWithEditableQuery.OrderBy(w => w.Warehouse.ApprovalStatus);
                        break;
                    case "createdat":
                        warehousesWithEditableQuery = firstSort.Dir.ToLower() == "desc" 
                            ? warehousesWithEditableQuery.OrderByDescending(w => w.Warehouse.CreatedAt)
                            : warehousesWithEditableQuery.OrderBy(w => w.Warehouse.CreatedAt);
                        break;
                    case "updatedat":
                        warehousesWithEditableQuery = firstSort.Dir.ToLower() == "desc" 
                            ? warehousesWithEditableQuery.OrderByDescending(w => w.Warehouse.UpdatedAt)
                            : warehousesWithEditableQuery.OrderBy(w => w.Warehouse.UpdatedAt);
                        break;
                    case "editable":
                        warehousesWithEditableQuery = firstSort.Dir.ToLower() == "desc" 
                            ? warehousesWithEditableQuery.OrderByDescending(w => w.Document.Editable)
                            : warehousesWithEditableQuery.OrderBy(w => w.Document.Editable);
                        break;
                    default:
                        warehousesWithEditableQuery = warehousesWithEditableQuery.OrderBy(w => w.Warehouse.CreatedAt);
                        break;
                }
            }
            else
            {
                // Default sorting if no sorts specified
                warehousesWithEditableQuery = warehousesWithEditableQuery.OrderBy(w => w.Warehouse.CreatedAt);
            }

            // Get total count before pagination
            var totalCount = await warehousesWithEditableQuery.CountAsync();

            // Apply pagination and create DTOs with Editable field
            var warehousesWithEditable = await warehousesWithEditableQuery
                .Skip(request.PagedData.Skip)
                .Take(request.PagedData.Take)
                .ToListAsync();

            // Map to WarehouseResponseDto with Editable field populated
            var warehouses = warehousesWithEditable.Select(wh => 
            {
                var dto = _mapper.Map<WarehouseResponseDto>(wh.Warehouse);
                // Create a new record with the Editable property set
                return dto with { Editable = wh.Document.Editable };
            }).ToList();

            var pageData = new PagedData
            {
                Skip = request.PagedData.Skip,
                Take = request.PagedData.Take,
                TotalCount = totalCount
            };

            return new ApiResponseDto<List<WarehouseResponseDto>>
            {
                Data = warehouses,
                PageData = pageData,
                Message = "Warehouses retrieved successfully",
                Success = true,
                Timestamp = DateTime.UtcNow,
            };
        }

        public async Task<WarehouseResponseDto?> GetByIdAsync(Guid id)
        {
            var warehouse = await _wareHouseRepository.FindOneAsync(x => x.Id == id);
            return warehouse == null ? null : _mapper.Map<WarehouseResponseDto>(warehouse);
        }

        public async Task<WarehouseResponseDto> CreateAsync(WarehouseRequestDto request)
        {
            var userExists = await _dbContext.Users.AnyAsync(u => u.Id == request.CreatedBy);

            if (!userExists)
            {
                throw new Exception($"User with ID {request.CreatedBy} does not exist");
            }

            var wareHouse = _mapper.Map<Warehouse>(request);
            wareHouse.Id = Guid.NewGuid();
            wareHouse.CreatedAt = DateTime.UtcNow;

            var feature = await _dbContext.ApprovalFeatures
                .Include(f => f.ApprovalConfig)
                .Include(f => f.ApprovalSteps)
                .Where(f => f.Status &&
                           f.ApprovalConfig != null &&
                           f.ApprovalConfig.ApprovalGroup == ApprovalGroup.Warehouse &&
                           f.ApprovalConfig.Status)
                .FirstOrDefaultAsync();

            if (feature == null)
            {
                throw new Exception("No active approval feature configured for Warehouse approval group. Please configure an approval workflow first.");
            }

            await _dbContext.Warehouses.AddAsync(wareHouse);

            // Tao ban ghi document
            var existedDocument = await _approvalDocumentRepository.FindOneAsync(a => a.ApprovalFeatureId == feature.Id && a.DocumentId == wareHouse.Id && a.ApprovalStatus != ApprovalStatus.Rejected);

            if (existedDocument != null)
            {
                throw new Exception("An active approval document already exists for this feature and data item");
            }
            var isDraft = request.ApprovalStatus == ApprovalStatus.Draft;
            var document = new ApprovalDocument
            {
                Id = Guid.NewGuid(),
                ApprovalFeatureId = feature.Id,
                DocumentId = wareHouse.Id,
                CurrentStepOrder = 1,
                ApprovalStatus = request.ApprovalStatus,
                Editable = isDraft,
                SubmittedAt = isDraft ? null : DateTime.UtcNow,
                SubmittedBy = isDraft ? null : request.CreatedBy,
                SubmitterId = isDraft ? null : request.CreatedBy,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow,
            };
            await _dbContext.ApprovalDocuments.AddAsync(document);

            // Tao lich su duyet
            var firstStep = feature.ApprovalSteps.OrderBy(x => x.StepOrder).FirstOrDefault();
            if (firstStep == null)
            {
                throw new Exception("Have no step here");
            }

            var history = new ApprovalHistory
            {
                Id = Guid.NewGuid(),
                ApprovalDocument = document,
                StepOrder = isDraft ? 0 : firstStep.StepOrder,
                ApproverId = isDraft ? null : firstStep.TargetId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedBy,
                SubmittedAt = isDraft ? null : DateTime.UtcNow,
                SubmittedBy = isDraft ? null : request.CreatedBy,
                Comment = string.Empty,
                ApprovalStatus = request.ApprovalStatus,
            };

            await _dbContext.ApprovalHistories.AddAsync(history);

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<WarehouseResponseDto>(wareHouse);
        }

        public async Task<bool> IsExistAsync(System.Linq.Expressions.Expression<Func<Warehouse, bool>> predicate)
        {
            return await _wareHouseRepository.FindOneAsync(predicate) != null;
        }

        public async Task<WarehouseResponseDto?> UpdateAsync(Guid id, WarehouseRequestDto request)
        {
            var wareHouse = await _wareHouseRepository
            .FindOneAsync(x => x.Id == id);
            if (wareHouse == null) return null;

            wareHouse.Name = request.Name;
            wareHouse.Status = request.Status;
            wareHouse.UpdatedAt = DateTime.UtcNow;
            wareHouse.UpdatedBy = request.UpdatedBy;

            _wareHouseRepository.UpdateOne(wareHouse);

            var approvalDocument = await _approvalDocumentRepository
                .FindOneAsync(document => document.DocumentId == wareHouse.Id);

            if (approvalDocument == null)
            {
                throw new Exception("Approval document not found for this warehouse");
            }

            var approvalHistory = await _approvalHistoryRepository
                .FindOneAsync(h => h.Id == approvalDocument.LatestApprovalHistoryId);

            if (approvalHistory != null && (approvalHistory.ApprovalStatus == ApprovalStatus.Draft || approvalHistory.ApprovalStatus == ApprovalStatus.Rejected))
            {
                approvalHistory.Id = Guid.NewGuid();
                approvalHistory.UpdatedAt = DateTime.UtcNow;
                approvalHistory.UpdatedBy = request.UpdatedBy;
                await _dbContext.ApprovalHistories.AddAsync(approvalHistory);
            }

            await _dbContext.SaveChangesAsync();
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

        public async Task<WarehouseResponseDto> UpdateStatusAsync(Guid id, UpdateApprovalRequestDto request)
        {
            var wareHouse = await _wareHouseRepository
                .FindOneAsync(x => x.Id == id);
            if (wareHouse == null) throw new Exception("Warehouse not found");

            wareHouse.ApprovalStatus = request.ApprovalStatus;
            wareHouse.UpdatedAt = DateTime.UtcNow;
            wareHouse.UpdatedBy = request.UpdatedBy;

            var document = new ApprovalHandlerRequestDto
            {
                ApprovalStatus = request.ApprovalStatus,
                ApproverId = request.UpdatedBy ?? Guid.Empty,
                DocumentId = wareHouse.Id,
                Comment = request.Comment
            };

            await ApprovalDocumentHandler(wareHouse, request);
            _dbContext.Warehouses.Update(wareHouse);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<WarehouseResponseDto>(wareHouse);
        }

        public async Task ApprovalDocumentHandler(Warehouse wareHouse, UpdateApprovalRequestDto request)
        {
            // Tim document
            var document = await _dbContext.ApprovalDocuments
                .Include(document => document.ApprovalHistories)
                .FirstAsync(document => document.DocumentId == wareHouse.Id);

            if (document == null)
            {
                throw new Exception("Document not found!");
            }

            // Tim history cua document
            var documentHistory = document.ApprovalHistories
                .Where(history => history.ApprovalStatus == ApprovalStatus.Pending && history.ApproverId == request.UpdatedBy)
                .OrderByDescending(history => history.CreatedAt)
                .FirstOrDefault();

            if (documentHistory == null)
            {
                throw new Exception("You haven't permission approve");
            }

            // Kiem tra xem con buoc tiep theo khong
            var currentFeature = await _dbContext.ApprovalFeatures
                .Include(feature => feature.ApprovalSteps)
                .FirstAsync(feature => feature.Id == document.ApprovalFeatureId && feature.Status);
            if (currentFeature == null)
            {
                throw new Exception("Feature not found!");
            }

            var newDocumentHistory = new ApprovalHistory
            {
                Id = Guid.NewGuid(),
                ApprovalDocument = document,
                CreatedAt = documentHistory.CreatedAt,
                CreatedBy = documentHistory.CreatedBy,
                SubmittedAt = documentHistory.SubmittedAt,
                SubmittedBy = documentHistory.SubmittedBy,
                Comment = request.Comment,
                ApprovalStatus = ApprovalStatus.Pending,
            };

            var nextStep = currentFeature.ApprovalSteps
                .FirstOrDefault(step => step.StepOrder > documentHistory.StepOrder);

            if (request.ApprovalStatus == ApprovalStatus.Rejected)
            {
                document.ApprovalStatus = ApprovalStatus.Rejected;
                wareHouse.ApprovalStatus = ApprovalStatus.Rejected;
                document.Editable = true;
                newDocumentHistory.ApprovalStatus = ApprovalStatus.Rejected;
            }
            else if (request.ApprovalStatus == ApprovalStatus.Approved)
            {
                if (nextStep != null)
                {
                    document.CurrentStepOrder = nextStep.StepOrder;
                    document.ApprovalStatus = ApprovalStatus.Pending;
                    wareHouse.ApprovalStatus = ApprovalStatus.Pending;
                    newDocumentHistory.StepOrder = nextStep.StepOrder;
                    newDocumentHistory.ApproverId = nextStep.TargetId;
                    newDocumentHistory.ApprovedAt = DateTime.UtcNow;
                    document.Editable = true;
                    newDocumentHistory.ApprovalStatus = ApprovalStatus.Pending;
                }
                else
                {
                    document.ApprovalStatus = ApprovalStatus.Approved;
                    wareHouse.ApprovalStatus = ApprovalStatus.Approved;
                    document.Editable = false;
                    newDocumentHistory.ApprovalStatus = ApprovalStatus.Approved;
                }
            }
            else
            {
                document.ApprovalStatus = ApprovalStatus.Draft;
                wareHouse.ApprovalStatus = ApprovalStatus.Draft;
                document.Editable = true;
                newDocumentHistory.ApprovalStatus = ApprovalStatus.Draft;
                newDocumentHistory.SubmittedAt = null;
                newDocumentHistory.SubmittedBy = null;
                newDocumentHistory.StepOrder = 1;
                newDocumentHistory.ApproverId = null;
                newDocumentHistory.ApprovedAt = null;
            }

            _dbContext.ApprovalDocuments.Update(document);
            await _dbContext.ApprovalHistories.AddAsync(newDocumentHistory);
        }
    }
}