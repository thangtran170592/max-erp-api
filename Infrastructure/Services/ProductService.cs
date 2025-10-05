using System.Linq.Expressions;
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
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _repositoryProduct;
        private readonly ApplicationDbContext _dbContext;
        private readonly IGenericRepository<ProductHistory> _repositoryProductHistory;
        private readonly IMapper _mapper;

        public ProductService(IGenericRepository<Product> repository, IGenericRepository<ProductHistory> repositoryProductHistory,
        IMapper mapper, ApplicationDbContext dbContext)
        {
            _repositoryProduct = repository;
            _repositoryProductHistory = repositoryProductHistory;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            var entities = await _repositoryProduct.FindAllAsync();
            return _mapper.Map<IEnumerable<ProductResponseDto>>(entities);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetManyAsync(Expression<Func<Product, bool>> predicate)
        {
            var entities = await _repositoryProduct.FindManyAsync(predicate);
            return _mapper.Map<IEnumerable<ProductResponseDto>>(entities);
        }

        public async Task<ApiResponseDto<List<ProductResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request)
        {
            var result = await _repositoryProduct.FindManyWithPagingAsync(request, [
                static x => x.PackageUnit!,
                static x => x.PackageUnit!.Package!,
                static x => x.PackageUnit!.Unit!,
                static x => x.Category!
            ]);
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
            var entity = await _repositoryProduct.FindOneAsync(x => x.Id == id);
            return entity == null ? null : _mapper.Map<ProductResponseDto>(entity);
        }

        public async Task<ProductResponseDto> CreateAsync(ProductRequestDto request)
        {
            var entity = _mapper.Map<Product>(request);
            entity.Id = Guid.NewGuid();
            entity.ApprovalStatus = ApprovalStatus.Pending;
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = request.CreatedBy;
            await _repositoryProduct.AddOneAsync(entity);

            var productHistory = _mapper.Map<ProductHistory>(entity);
            productHistory.Id = Guid.NewGuid();
            productHistory.ProductId = entity.Id;
            await _repositoryProductHistory.AddOneAsync(productHistory);

            await _repositoryProduct.SaveChangesAsync();
            await LoadRelatedEntitiesAsync(entity);
            return _mapper.Map<ProductResponseDto>(entity);
        }

        public async Task<ProductResponseDto?> UpdateAsync(Guid id, ProductRequestDto request)
        {
            var entity = await _repositoryProduct.FindOneAsync(x => x.Id == id);
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

            _repositoryProduct.UpdateOne(entity);

            var productHistory = _mapper.Map<ProductHistory>(entity);
            productHistory.Id = Guid.NewGuid();
            productHistory.Uid = entity.Uid;
            productHistory.ProductId = entity.Id;
            productHistory.Status = request.Status;
            productHistory.UpdatedAt = DateTime.UtcNow;
            productHistory.UpdatedBy = request.UpdatedBy;
            await _repositoryProductHistory.AddOneAsync(productHistory);

            await _repositoryProduct.SaveChangesAsync();
            await LoadRelatedEntitiesAsync(entity);
            return _mapper.Map<ProductResponseDto>(entity);
        }

        public async Task<int> DeleteAsync(Guid id, Guid deletedBy)
        {
            var entity = await _repositoryProduct.FindOneAsync(x => x.Id == id);
            if (entity == null) return 0;
            _repositoryProduct.DeleteOne(entity);
            var result = await _repositoryProduct.SaveChangesAsync();
            return result;
        }

        public async Task<ProductResponseDto> UpdateStatusAsync(Guid id, UpdateProductStatusRequestDto request)
        {
            var product = await _repositoryProduct.FindOneAsync(x => x.Id == id);
            if (product == null) throw new Exception("Product not found");

            product.ApprovalStatus = request.ApprovalStatus;
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = request.UpdatedBy;

            _repositoryProduct.UpdateOne(product);

            var productHistory = _mapper.Map<ProductHistory>(product);
            productHistory.Id = Guid.NewGuid();
            productHistory.Uid = product.Uid;
            productHistory.ProductId = product.Id;
            productHistory.ReasonRejection = request.ReasonRejection;
            productHistory.ApprovalStatus = request.ApprovalStatus;
            productHistory.UpdatedAt = DateTime.UtcNow;
            productHistory.UpdatedBy = request.UpdatedBy;

            await _repositoryProductHistory.AddOneAsync(productHistory);

            await _repositoryProduct.SaveChangesAsync();
            await LoadRelatedEntitiesAsync(product);
            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<bool> IsExistAsync(Expression<Func<Product, bool>> predicate)
            => await _repositoryProduct.FindOneAsync(predicate) != null;

        public async Task<IEnumerable<ProductHistoryResponseDto>> GetProductHistoryAsync(Guid productId)
        {
            var productHistoriesQuery = from productHistory in _dbContext.ProductHistories
                                        where productHistory.ProductId == productId
                                        join creatorUser in _dbContext.Users on productHistory.CreatedBy equals creatorUser.Id into createdGroup
                                        from createdUser in createdGroup.DefaultIfEmpty()
                                        join updatedUser in _dbContext.Users on productHistory.UpdatedBy equals updatedUser.Id into updatedGroup
                                        from updatedUser in updatedGroup.DefaultIfEmpty()
                                        select new ProductHistoryResponseDto
                                        {
                                            ProductId = productHistory.ProductId,
                                            Uid = productHistory.Uid,
                                            Name = productHistory.Name,
                                            CategoryId = productHistory.CategoryId,
                                            CategoryName = productHistory.Category != null ? productHistory.Category.Name : string.Empty,
                                            Price = productHistory.Price,
                                            Length = productHistory.Length,
                                            Width = productHistory.Width,
                                            Height = productHistory.Height,
                                            ReasonRejection = productHistory.ReasonRejection,
                                            Status = productHistory.Status,
                                            ApprovalStatus = productHistory.ApprovalStatus,
                                            CreatedAt = productHistory.CreatedAt,
                                            CreatedBy = productHistory.CreatedBy,
                                            UpdatedAt = productHistory.UpdatedAt,
                                            UpdatedBy = productHistory.UpdatedBy,
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
            var histories = await productHistoriesQuery.AsNoTracking().ToListAsync();
            if (histories == null) throw new Exception("Product not found");
            return histories;
        }

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