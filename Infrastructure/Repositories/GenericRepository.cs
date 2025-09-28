using System.Linq.Expressions;
using System.Reflection;
using Application.Common.Models;
using Application.IRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();

        }

        public async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[]? includes, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;
            if (includes != null && includes.Any())
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.Where(predicate).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PagedResult<TEntity>> FindManyWithPagingAsync(
            Dictionary<string, object>? filters = null,
            int page = 1,
            int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            IQueryable<TEntity> query = _dbSet.AsQueryable();

            // Dynamic filter
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    var property = typeof(TEntity).GetProperty(filter.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (property == null) continue;

                    var parameter = Expression.Parameter(typeof(TEntity), "x");
                    var member = Expression.Property(parameter, property);
                    var constant = Expression.Constant(filter.Value);

                    Expression? body;

                    if (property.PropertyType == typeof(string))
                    {
                        // x.Property != null && x.Property.Contains(filter.Value)
                        var notNull = Expression.NotEqual(member, Expression.Constant(null, typeof(string)));
                        var contains = Expression.Call(
                            member,
                            typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!,
                            constant
                        );
                        body = Expression.AndAlso(notNull, contains);
                    }
                    else
                    {
                        // x.Property == filter.Value
                        body = Expression.Equal(member, constant);
                    }

                    var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);
                    query = query.Where(lambda);
                }
            }
            var totalCount = await query.CountAsync();

            // Paging
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<TEntity>
            {
                Data = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[]? includes, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;
            if (includes != null && includes.Any())
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>>[]? filters,
         Expression<Func<TEntity, object>>[]? includes,
          CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;
            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }
            if (includes != null && includes.Any())
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task AddOneAsync(TEntity entity, CancellationToken cancellationToken = default) =>
            await _dbSet.AddAsync(entity, cancellationToken);

        public async Task AddManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) =>
            await _dbSet.AddRangeAsync(entities, cancellationToken);

        public virtual EntityEntry<TEntity> UpdateOne(TEntity entity) =>
            _dbSet.Update(entity);

        public void UpdateMany(IEnumerable<TEntity> entities) =>
            _dbSet.UpdateRange(entities);

        public void DeleteOne(TEntity entity) => _dbSet.Remove(entity);

        public void DeleteMany(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}