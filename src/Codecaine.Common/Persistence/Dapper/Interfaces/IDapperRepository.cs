using Codecaine.Common.Domain;
using Codecaine.Common.Pagination.Interfaces;
using Codecaine.Common.Primitives.Maybe;

namespace Codecaine.Common.Persistence.Dapper.Interfaces
{
    public interface IDapperRepository<TEntity>  where TEntity : Entity
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
        Task Insert(TEntity entity);

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        Task Update(TEntity entity);

        /// <summary>
        /// Deletes an entity from the repository by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        Task Delete(Guid id);

        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        Task Remove(TEntity entity);

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

        Task<IEnumerable<(string Content, double Similarity)>> SearchContentVectorAsync(string input, int topK = 5);

        Task<IEnumerable<(Guid id, double Similarity)>> SearchIdByVectorAsync(string input, int topK = 5);

        Task<IEnumerable<(TEntity entity, double Similarity)>> SearchEntityByVectorAsync(string input, int topK = 5);

    }
}
