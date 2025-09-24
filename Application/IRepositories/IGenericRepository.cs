using System.Linq.Expressions;
using Application.Common.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
namespace Application.IRepositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<PagedResult<TEntity>> FindManyWithPagingAsync(Dictionary<string, object>? filters = null, int page = 1, int pageSize = 10);
        Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[]? include = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[]? include = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>[]? filters = null, Expression<Func<TEntity, object>>[]? includes = null, CancellationToken cancellationToken = default);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        EntityEntry<TEntity> Update(TEntity entity);
        public void UpdateRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}