using Codecaine.Common.Domain;
using Codecaine.Common.Pagination.Interfaces;
using Codecaine.Common.Primitives.Maybe;

namespace Codecaine.Common.Persistence
{
    /// <summary>
    /// Defines a repository interface for managing entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IRepository<TEntity>
       where TEntity : Entity
    {
        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Maybe{TEntity}"/>.</returns>
        Task<Maybe<TEntity>> GetByIdAsync(Guid id);

        /// <summary>
        /// Inserts a new entity into the repository.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        void Insert(TEntity entity);

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Deletes an entity from the repository by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        void Delete(Guid id);

        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Remove(TEntity entity);        

        /// <summary>
        /// Retrieves a paginated result of entities that match the specified filter and sorting criteria.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="itemsPerPage">The number of items per page.</param>
        /// <param name="orderBy">The property to order by.</param>
        /// <param name="isDecending">Indicates whether the sorting should be in descending order.</param>
        /// <param name="filter">The filter criteria for querying entities.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PagedResult{TEntity}"/>.</returns>
       
        Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        ISpecification<TEntity>? specification = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default);

        Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(Specification<TEntity> spec, CancellationToken cancellationToken = default);
    }
}
