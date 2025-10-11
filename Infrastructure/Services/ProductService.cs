using System.Linq.Expressions;
using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Core.Entities;
using Core.Enums;
using Infrastructure.Data;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProductService(IGenericRepository<Product> productRepository,
        IMapper mapper, ApplicationDbContext dbContext)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            var entities = await _productRepository.FindAllAsync();
            return _mapper.Map<IEnumerable<ProductResponseDto>>(entities);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetManyAsync(Expression<Func<Product, bool>> predicate)
        {
            var entities = await _productRepository.FindManyAsync(predicate);
            return _mapper.Map<IEnumerable<ProductResponseDto>>(entities);
        }

        public async Task<ApiResponseDto<List<ProductResponseDto>>> GetManyWithPagingAsync(Guid userId, Guid? departmentId, Guid? positionId, FilterRequestDto request)
        {
            var result = await _productRepository.FindManyWithPagingAsync(request, includes: [
                static x => x.PackageUnit!,
                static x => x.PackageUnit!.Package!,
                static x => x.PackageUnit!.Unit!,
                static x => x.Category!
            ], predicate: x =>
                // Product join với ApprovalDocument và có approval history pending cho user hiện tại
                _dbContext.ApprovalDocuments.Any(doc => 
                    doc.DocumentId == x.Id && 
                    doc.ApprovalStatus == ApprovalStatus.Pending &&
                    doc.ApprovalHistories.Any(history => 
                        (history.ApproverId == userId || 
                         history.ApproverId == departmentId || 
                         history.ApproverId == positionId) && 
                        history.ApprovalStatus == ApprovalStatus.Pending))
            );

            return new ApiResponseDto<List<ProductResponseDto>>
            {
                Data = _mapper.Map<List<ProductResponseDto>>(result.Data),
                PageData = result.PageData,
                Message = result.Message,
                Success = result.Success,
                Timestamp = result.Timestamp,
            };
        }

        public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
        {
            var entity = await _productRepository.FindOneAsync(x => x.Id == id);
            return entity == null ? null : _mapper.Map<ProductResponseDto>(entity);
        }

        public async Task<ProductResponseDto> CreateAsync(ProductRequestDto request)
        {
            var entity = _mapper.Map<Product>(request);
            entity.Id = Guid.NewGuid();
            entity.ApprovalStatus = ApprovalStatus.Pending;
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = request.CreatedBy;
            await _productRepository.AddOneAsync(entity);

            var feature = await _dbContext.ApprovalFeatures
                .Include(x => x.ApprovalConfig)
                .Where(f => f.Status && f.ApprovalConfig != null && f.ApprovalConfig.ApprovalGroup == ApprovalGroup.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var document = new InitializeApprovalRequestDto
            {
                ApprovalGroup = ApprovalGroup.Product,
                ApprovalFeatureId = feature?.Id ?? Guid.Empty,
                DocumentId = entity.Id,
            };

            await ApprovalHandleHelper.InitializeApprovalProcess(_dbContext, document);
            await _dbContext.SaveChangesAsync();
            await LoadRelatedEntitiesAsync(entity);
            return _mapper.Map<ProductResponseDto>(entity);
        }

        public async Task<ProductResponseDto?> UpdateAsync(Guid id, ProductRequestDto request)
        {
            var entity = await _productRepository.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;

            entity.Uid = request.Uid;
            entity.Name = request.Name;
            entity.Price = request.Price;
            entity.Length = request.Length;
            entity.Width = request.Width;
            entity.Height = request.Height;
            entity.Status = request.Status;
            entity.CategoryId = request.CategoryId;
            entity.PackageUnitId = request.PackageUnitId;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = request.UpdatedBy;

            _productRepository.UpdateOne(entity);

            // var approvalHistory = entity
            //    .OrderByDescending(h => h.CreatedAt)
            //    .FirstOrDefault();

            // if (approvalHistory != null && (approvalHistory.Status == ApprovalStatus.Draft || approvalHistory.Status == ApprovalStatus.Rejected))
            // {
            //     approvalHistory.Id = Guid.NewGuid();
            //     approvalHistory.UpdatedAt = DateTime.UtcNow;
            //     approvalHistory.UpdatedBy = request.UpdatedBy;
            //     await _dbContext.ApprovalHistories.AddAsync(approvalHistory);
            // }

            await _dbContext.SaveChangesAsync();
            await LoadRelatedEntitiesAsync(entity);
            return _mapper.Map<ProductResponseDto>(entity);
        }

        public async Task<int> DeleteAsync(Guid id, Guid deletedBy)
        {
            var entity = await _productRepository.FindOneAsync(x => x.Id == id);
            if (entity == null) return 0;
            _productRepository.DeleteOne(entity);
            var result = await _productRepository.SaveChangesAsync();
            return result;
        }

        public async Task<ProductResponseDto> UpdateStatusAsync(Guid id, UpdateApprovalRequestDto request)
        {
            var product = await _productRepository.FindOneAsync(x => x.Id == id);
            if (product == null) throw new Exception("Product not found");

            product.ApprovalStatus = request.ApprovalStatus;
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = request.UpdatedBy;

            _productRepository.UpdateOne(product);

            var document = new ApprovalHandlerRequestDto
            {
                ApprovalStatus = request.ApprovalStatus,
                ApproverId = request.UpdatedBy ?? Guid.Empty,
                DocumentId = product.Id,
                Comment = request.Comment
            };

            await ApprovalHandleHelper.ApprovalDocumentHandler(_dbContext, document);
            await _dbContext.SaveChangesAsync();
            await LoadRelatedEntitiesAsync(product);
            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<bool> IsExistAsync(Expression<Func<Product, bool>> predicate)
            => await _productRepository.FindOneAsync(predicate) != null;

        private async Task LoadRelatedEntitiesAsync(Product entity)
        {
            if (entity == null) return;
            await _dbContext.Entry(entity).Reference(x => x.PackageUnit).LoadAsync();
            if (entity.PackageUnit != null)
            {
                await _dbContext.Entry(entity.PackageUnit).Reference(x => x.Package).LoadAsync();
                await _dbContext.Entry(entity.PackageUnit).Reference(x => x.Unit).LoadAsync();
            }
            await _dbContext.Entry(entity).Reference(x => x.Category).LoadAsync();
        }
    }
}