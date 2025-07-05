using Codecaine.Common.Domain;
using Codecaine.Common.Primitives.Maybe;
using System.Data;

namespace Codecaine.Common.Persistence.Dapper.Interfaces
{
    public interface IDapperDbContext
    {
        IDbConnection Connection { get; }
        IDbTransaction? Transaction { get; }

        /// <summary>
        /// Gets the entity with the specified identifier.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="id">The entity identifier.</param>
        /// <returns>The <typeparamref name="TEntity"/> with the specified identifier if it exists, otherwise null.</returns>
        Task<Maybe<TEntity>> GetBydIdAsync<TEntity>(Guid id)
            where TEntity : Entity;

        /// <summary>
        /// Inserts the specified entity into the database.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entity">The entity to be inserted into the database.</param>
        Task Insert<TEntity>(TEntity entity)
            where TEntity : Entity;

        /// <summary>
        /// Inserts the specified entities into the database.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entities">The entities to be inserted into the database.</param>
        Task InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities)
            where TEntity : Entity;

        /// <summary>
        /// Removes the specified entity from the database.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entity">The entity to be removed from the database.</param>
        Task Remove<TEntity>(TEntity entity)
            where TEntity : Entity;

        Task Update<TEntity>(TEntity entity) where TEntity : Entity;

        Task<IEnumerable<(string Content, double Similarity)>> SearchContentVectorAsync<TEntity>(string input, int topK = 5) where TEntity : Entity;

        Task<IEnumerable<(Guid id, double Similarity)>> SearchIdByVectorAsync<TEntity>(string input, int topK = 5) where TEntity : Entity;

        Task<IEnumerable<(TEntity entity, double Similarity)>> SearchEntityByVectorAsync<TEntity>(string input, int topK = 5) where TEntity : Entity;



    }
}
