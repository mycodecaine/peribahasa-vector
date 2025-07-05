using Codecaine.Common.Domain;
using Codecaine.Common.Primitives.Maybe;
using MongoDB.Driver;
using static Amazon.S3.Util.S3EventNotification;

namespace Codecaine.Common.Persistence.MongoDB.Interfaces
{
    public interface IMongoDbContext: INoSqlUnitOfWork
    {
        IMongoCollection<T> GetCollection<T>(string name);
        Task<IClientSessionHandle> StartSessionAsync();       

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
        void Insert<TEntity>(TEntity entity)
            where TEntity : Entity;

        /// <summary>
        /// Inserts the specified entities into the database.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entities">The entities to be inserted into the database.</param>
        void InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities)
            where TEntity : Entity;

        /// <summary>
        /// Removes the specified entity from the database.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entity">The entity to be removed from the database.</param>
        void Remove<TEntity>(TEntity entity)
            where TEntity : Entity;

        void Update<TEntity>(TEntity entity) where TEntity : Entity;

        IQueryable<T> AsQueryable<T>(string name);
    }
}
