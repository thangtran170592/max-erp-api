using System.Linq.Expressions;
using System.Reflection;
using Application.Common.Models;
using Application.Dtos;
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

        public async Task<ApiResponseDto<List<TEntity>>> FindManyWithPagingAsync(FilterRequestDto request)
        {
            if (request.PagedData.Skip < 0) request.PagedData.Skip = 0;
            if (request.PagedData.Take <= 0) request.PagedData.Take = 10;

            IQueryable<TEntity> query = _dbSet.AsQueryable();

            // Dynamic filter
            if (request.Filters != null && request.Filters.Any())
            {
                foreach (var filter in request.Filters)
                {
                    var property = typeof(TEntity).GetProperty(filter.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (property == null) continue;

                    if (property.PropertyType != typeof(string)) continue;

                    var parameter = Expression.Parameter(typeof(TEntity), "x");
                    var member = Expression.Property(parameter, property);

                    var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
                    var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
                    var value = filter.Value?.ToString()?.ToLower() ?? string.Empty;
                    var memberToLower = Expression.Call(member, toLowerMethod);
                    var constant = Expression.Constant(value);
                    var body = Expression.Call(memberToLower, containsMethod, constant);

                    var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);
                    query = query.Where(lambda);
                }
            }

            // Sorting
            if (request.Sorts != null && request.Sorts.Any())
            {
                IOrderedQueryable<TEntity>? orderedQuery = null;
                foreach (var sort in request.Sorts)
                {
                    var property = typeof(TEntity).GetProperty(sort.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (property == null) continue;
                    var parameter = Expression.Parameter(typeof(TEntity), "x");
                    var member = Expression.Property(parameter, property);
                    var converted = Expression.Convert(member, typeof(object));
                    var keySelector = Expression.Lambda<Func<TEntity, object>>(converted, parameter);

                    if (orderedQuery == null)
                    {
                        orderedQuery = sort.Dir == "desc"
                            ? Queryable.OrderByDescending(query, keySelector)
                            : Queryable.OrderBy(query, keySelector);
                    }
                    else
                    {
                        orderedQuery = sort.Dir == "desc"
                            ? Queryable.ThenByDescending(orderedQuery, keySelector)
                            : Queryable.ThenBy(orderedQuery, keySelector);
                    }
                }
                query = orderedQuery ?? query;
            }

            // Total count before paging
            var totalCount = await query.CountAsync();

            // Paging
            var items = await query
                .Skip(request.PagedData.Skip)
                .Take(request.PagedData.Take)
                .ToListAsync();

            return new ApiResponseDto<List<TEntity>>
            {
                Success = true,
                Message = "Data retrieved successfully",
                Timestamp = DateTime.UtcNow,
                Data = items,
                PageData = new PagedData
                {
                    TotalCount = totalCount,
                    Skip = request.PagedData.Skip,
                    Take = request.PagedData.Take
                }
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