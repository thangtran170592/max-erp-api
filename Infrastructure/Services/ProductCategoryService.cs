using System.Linq.Expressions;
using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Core.Entities;

namespace Infrastructure.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IGenericRepository<ProductCategory> _repository;
        private readonly IMapper _mapper;

        public ProductCategoryService(IGenericRepository<ProductCategory> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductCategoryResponseDto>> GetAllAsync()
        {
            var entities = await _repository.FindAllAsync();
            return _mapper.Map<IEnumerable<ProductCategoryResponseDto>>(entities);
        }

        public async Task<ApiResponseDto<List<ProductCategoryResponseDto>>> GetManyWithPagingAsync(FilterRequestDto request)
        {
            var result = await _repository.FindManyWithPagingAsync(request);
            return new ApiResponseDto<List<ProductCategoryResponseDto>>
            {
                Data = _mapper.Map<List<ProductCategoryResponseDto>>(result.Data),
                PageData = result.PageData,
                Message = result.Message,
                Success = result.Success
            };
        }

        public async Task<ProductCategoryResponseDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            return entity == null ? null : _mapper.Map<ProductCategoryResponseDto>(entity);
        }

        public async Task<ProductCategoryResponseDto> CreateAsync(ProductCategoryRequestDto request)
        {
            var entity = _mapper.Map<ProductCategory>(request);
            entity.Id = Guid.NewGuid();
            await _repository.AddOneAsync(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ProductCategoryResponseDto>(entity);
        }

        public async Task<ProductCategoryResponseDto?> UpdateAsync(Guid id, ProductCategoryRequestDto request)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return null;

            entity.Name = request.Name;
            entity.Status = request.Status;

            _repository.UpdateOne(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ProductCategoryResponseDto>(entity);
        }

        public async Task<ProductCategoryResponseDto> UpdateStatusAsync(Guid id, ProductCategoryStatusUpdateDto request)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id) ?? throw new KeyNotFoundException("ProductCategory not found");
            entity.Status = request.Status;
            _repository.UpdateOne(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ProductCategoryResponseDto>(entity);
        }

        public async Task<int> DeleteAsync(Guid id, Guid deletedBy)
        {
            var entity = await _repository.FindOneAsync(x => x.Id == id);
            if (entity == null) return 0;
            _repository.DeleteOne(entity);
            var result = await _repository.SaveChangesAsync();
            return result;
        }

        public async Task<bool> IsExistAsync(Expression<Func<ProductCategory, bool>> predicate)
            => await _repository.FindOneAsync(predicate) != null;
    }
}