using Codecaine.Common.Domain;
using Codecaine.Common.Pagination.Interfaces;
using Codecaine.Common.Persistence.EfCore.Interfaces;
using Codecaine.Common.Primitives.Maybe;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Codecaine.Common.Persistence.EfCore
{
    /// <summary>
    /// Represents a generic repository for managing entities in a relational database context.
    /// Provides methods for CRUD operations and querying entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        protected Repository(IDbContext dbContext) => DbContext = dbContext;

        protected IDbContext DbContext { get; }

        /// <summary>
        /// Deletes the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to delete.</param>
        public async void Delete(Guid id)
        {
            var entity = await DbContext.GetBydIdAsync<TEntity>(id);
            var data = entity.Value;
            Remove(data);
        }

        /// <summary>
        /// Gets the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to retrieve.</param>
        /// <returns>A maybe instance that may contain the entity with the specified identifier.</returns>
        public virtual async Task<Maybe<TEntity>> GetByIdAsync(Guid id) => await DbContext.GetBydIdAsync<TEntity>(id);

        /// <summary>
        /// Gets a queryable collection of entities that match the specified filter.
        /// </summary>
        /// <param name="filter">The filter to apply to the query.</param>
        /// <returns>A queryable collection of entities that match the specified filter.</returns>
        public async Task<IQueryable<TEntity>> GetQueryable(QueryFilter<TEntity> filter)
        {
            var dataQ = DbContext.Set<TEntity>().AsQueryable();
            var data = await Task.Run(() => dataQ.Where(filter.Combine()));
            return data;
        }

        /// <summary>
        /// Inserts the specified entity into the database.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        public void Insert(TEntity entity) => DbContext.Insert(entity);

        /// <summary>
        /// Updates the specified entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public void Update(TEntity entity) => DbContext.Set<TEntity>().Update(entity);

        /// <summary>
        /// Removes the specified entity from the database.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void Remove(TEntity entity) => DbContext.Remove(entity);

        /// <summary>
        /// Checks if any entity meets the specified specification.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <returns>True if any entity meets the specified specification, otherwise false.</returns>
        protected async Task<bool> AnyAsync(Specification<TEntity> specification) =>
            await DbContext.Set<TEntity>().AnyAsync(specification);

        /// <summary>
        /// Gets the first entity that meets the specified specification.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <returns>The maybe instance that may contain the first entity that meets the specified specification.</returns>
        protected async Task<Maybe<TEntity?>> FirstOrDefaultAsync(Specification<TEntity> specification)
        {
            return await DbContext.Set<TEntity>().FirstOrDefaultAsync(specification);
        }

        /// <summary>
        /// Gets a list of entities that meet the specified specification.
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        protected async Task<Maybe<List<TEntity>>> Where(Specification<TEntity> specification)
        {
            var (query,_) = ApplySpecification(specification);
            return await query.ToListAsync();
        }

        /// <summary>
        /// Gets a paginated list of entities based on the specified parameters.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="specification"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDescending"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, ISpecification<TEntity>? specification = null, string? sortBy = null, bool sortDescending = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = DbContext.Set<TEntity>();

            if (specification != null)
            {
                query = query.Where(specification.Criteria);
            }

            var skip = (page - 1) * pageSize;

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var parameter = Expression.Parameter(typeof(TEntity), "x");
                var property = Expression.Property(parameter, sortBy);
                var lambda = Expression.Lambda(property, parameter);

                string method = sortDescending ? "OrderByDescending" : "OrderBy";
                var methodCall = typeof(Queryable).GetMethods()
                    .First(m => m.Name == method && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(TEntity), property.Type);

                query = (IQueryable<TEntity>)methodCall.Invoke(null, new object[] { query, lambda })!;
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(Specification<TEntity> spec, CancellationToken cancellationToken = default)        {
            

            var (query, totalCount) = ApplySpecification(spec);

            var items = await query.ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        protected (IQueryable<TEntity>, int TotalCount) ApplySpecification(Specification<TEntity> spec)
        {
            IQueryable<TEntity> query = DbContext.Set<TEntity>();

            // Filter
            query = query.Where(spec.ToExpression());

            var totalCount =  query.Count();

            // Includes
            foreach (var include in spec.Includes)
                query = query.Include(include);

            if ((spec.PageNumber.HasValue || spec.PageSize.HasValue) && string.IsNullOrEmpty(spec.OrderByString) && string.IsNullOrEmpty(spec.OrderByDescendingString))
            {
                query = query.OrderBy("Id"); // or whatever is your default ordering key
            }

            if (!string.IsNullOrEmpty(spec.OrderByString))
            {
                query = query.OrderBy(spec.OrderByString); // Uses Dynamic LINQ
            }
            else if (!string.IsNullOrEmpty(spec.OrderByDescendingString))
            {
                query = query.OrderBy($"{spec.OrderByDescendingString} descending");
            }

            if (spec.PageNumber.HasValue)
            {
                var skip = (spec.PageNumber.Value - 1) * (spec.PageSize.HasValue ? spec.PageSize.Value : 1);
                query = query.Skip(skip);
            }

            if (spec.PageSize.HasValue)
            {
                query = query.Take(spec.PageSize.Value);
            }            

            return (query, totalCount);
        }
    }
}
