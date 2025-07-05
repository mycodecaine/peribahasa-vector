using Codecaine.Common.Domain;
using Codecaine.Common.Pagination.Interfaces;
using Codecaine.Common.Persistence.MongoDB.Interfaces;
using Codecaine.Common.Primitives.Maybe;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using MongoDB.Driver.Linq;


namespace Codecaine.Common.Persistence.MongoDB
{
    public class MongoRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        
        protected IMongoDbContext Context { get; }

        public MongoRepository(IMongoDbContext context)
        {
          
            Context = context;
        }
        public void Delete(Guid id)
        {
            var data = Context.GetBydIdAsync<TEntity>(id).Result;
            if (data.HasValue)
            {
                Remove(data.Value);
            }
        }

        public async Task<Maybe<TEntity>> GetByIdAsync(Guid id)
        {
            return await Context.GetBydIdAsync<TEntity>(id);
        }

       

        public void Insert(TEntity entity)
        {
            Context.Insert(entity);
        }

        public void Remove(TEntity entity)
        {
            Context.Remove(entity);
        }

        public void Update(TEntity entity)
        {
            Context.Update(entity);
        }

        protected async Task<bool> AnyAsync(Specification<TEntity> specification) =>
            await Context.AsQueryable<TEntity>(typeof(TEntity).Name).AnyAsync(specification);

        protected async Task<Maybe<TEntity?>> FirstOrDefaultAsync(Specification<TEntity> specification)
        {
            return await Context.AsQueryable<TEntity>(typeof(TEntity).Name).FirstOrDefaultAsync(specification);
        }

        protected async Task<Maybe<List<TEntity>>> Where(Specification<TEntity> specification)
        {
            return await Context.AsQueryable<TEntity>(typeof(TEntity).Name).Where(specification).ToListAsync();
        }

        public async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(Specification<TEntity> spec, CancellationToken cancellationToken = default)
        {
            var (query, totalCount) = ApplySpecification(spec);

            var items = await query.ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, ISpecification<TEntity>? specification = null, string? sortBy = null, bool sortDescending = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = Context.AsQueryable<TEntity>(typeof(TEntity).Name);

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

        protected (IQueryable<TEntity>, int TotalCount) ApplySpecification(Specification<TEntity> spec)
        {
            IQueryable<TEntity> query = Context.AsQueryable<TEntity>(typeof(TEntity).Name);
            // Filter
            query = query.Where(spec.ToExpression());

            var totalCount = query.Count();           

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
