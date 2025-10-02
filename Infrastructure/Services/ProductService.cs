using System.Linq.Expressions;
using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Core.Entities;

namespace Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _repository;
        private readonly IMapper _mapper;

        public ProductService(IGenericRepository<Product> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            var entities = await _repository.FindAllAsync();
            return _mapper.Map<IEnumerable<ProductResponseDto>>(entities);
        }

        public async Task<ApiResponseDto<List<ProductResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request)
        {
            var result = await _repository.FindManyWithPagingAsync(request);
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
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            return entity == null ? null : _mapper.Map<ProductResponseDto>(entity);
        }

        public async Task<ProductResponseDto> CreateAsync(ProductRequestDto request)
        {
            var entity = _mapper.Map<Product>(request);
            entity.Id = Guid.NewGuid();
            await _repository.AddOneAsync(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ProductResponseDto>(entity);
        }

        public async Task<ProductResponseDto?> UpdateAsync(Guid id, ProductRequestDto request)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;

            entity.Uid = request.Uid;
            entity.Name = request.Name;
            entity.Price = request.Price;
            entity.Status = request.Status;
            entity.CategoryId = request.CategoryId;
            entity.PackageId = request.PackageId;
            entity.UnitOfMeasureId = request.UnitOfMeasureId;

            _repository.UpdateOne(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ProductResponseDto>(entity);
        }

        public async Task<ProductResponseDto> UpdateStatusAsync(Guid id, ProductStatusUpdateDto request)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id) ?? throw new KeyNotFoundException("Product not found");
            entity.Status = request.Status;
            _repository.UpdateOne(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ProductResponseDto>(entity);
        }

        public async Task<int> DeleteAsync(Guid id, Guid deletedBy)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return 0;
            _repository.DeleteOne(entity);
            var result = await _repository.SaveChangesAsync();
            return result;
        }

        public async Task<bool> IsExistAsync(Expression<Func<Product, bool>> predicate)
            => await _repository.FindOneAsync(predicate) != null;
    }
}