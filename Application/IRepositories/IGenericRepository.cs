using System.Linq.Expressions;
using Application.Dtos;
using Microsoft.EntityFrameworkCore.ChangeTracking;
namespace Application.IRepositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<ApiResponseDto<List<TEntity>>> FindManyWithPagingAsync(FilterRequestDto request);
        Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[]? include = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[]? include = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>>[]? filters = null, Expression<Func<TEntity, object>>[]? includes = null, CancellationToken cancellationToken = default);
        Task AddOneAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task AddManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        EntityEntry<TEntity> UpdateOne(TEntity entity);
        public void UpdateMany(IEnumerable<TEntity> entities);
        void DeleteOne(TEntity entity);
        void DeleteMany(IEnumerable<TEntity> entities);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
