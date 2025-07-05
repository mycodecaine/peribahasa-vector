using Codecaine.Common.Abstractions;
using Codecaine.Common.Domain;
using Codecaine.Common.Domain.Events;
using Codecaine.Common.Domain.Interfaces;
using Codecaine.Common.Persistence.EfCore.Extensions;
using Codecaine.Common.Persistence.EfCore.Interfaces;
using Codecaine.Common.Primitives.Maybe;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;

namespace Codecaine.Common.Persistence.EfCore
{
    public abstract class AppDbContext : DbContext, IDbContext, IUnitOfWork
    {
        private readonly IDateTime _dateTime;
        private readonly IMediator _mediator;
        protected AppDbContext()
        {
        }
        protected AppDbContext(DbContextOptions options)
             : base(options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventReminderDbContext"/> class.
        /// </summary>
        /// <param name="options">The database context options.</param>
        /// <param name="dateTime">The current date and time.</param>
        /// <param name="mediator">The mediator.</param>
        protected AppDbContext(DbContextOptions options, IDateTime dateTime, IMediator mediator)
            : base(options)
        {
            _dateTime = dateTime;
            _mediator = mediator;
        }


        public new DbSet<TEntity> Set<TEntity>()
            where TEntity : Entity
            => base.Set<TEntity>();

        /// <inheritdoc />
        public async Task<Maybe<TEntity>> GetBydIdAsync<TEntity>(Guid id)
            where TEntity : Entity
        {

            return id == Guid.Empty ?
                        Maybe<TEntity>.None :
                        Maybe<TEntity>.From(await Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id));
        }

        /// <inheritdoc />
        public void Insert<TEntity>(TEntity entity)
            where TEntity : Entity
            => Set<TEntity>().Add(entity);

        /// <inheritdoc />
        public void InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities)
            where TEntity : Entity
            => Set<TEntity>().AddRange(entities);

        /// <inheritdoc />
        public new void Remove<TEntity>(TEntity entity)
            where TEntity : Entity
            => Set<TEntity>().Remove(entity);

        
        /// <summary>
        /// Saves all of the pending changes in the unit of work.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of entities that have been saved.</returns>
        public async Task<int> SaveChangesAsync(Guid saveBy, CancellationToken cancellationToken = default)
        {
            DateTime utcNow = _dateTime.UtcNow;
            UpdateAuditableEntities(saveBy, utcNow);

            UpdateSoftDeletableEntities(saveBy, utcNow);
            await PublishDomainEvents(cancellationToken);

            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
            => Database.BeginTransactionAsync(cancellationToken);

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.ApplyUtcDateTimeConverter();

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Updates the entities implementing <see cref="IAuditableEntity"/> interface.
        /// </summary>
        /// <param name="utcNow">The current date and time in UTC format.</param>
        private void UpdateAuditableEntities(Guid saveBy, DateTime utcNow)
        {
            foreach (EntityEntry<IAuditableEntity> entityEntry in ChangeTracker.Entries<IAuditableEntity>())
            {
                if (entityEntry.State == EntityState.Added)
                {
                    entityEntry.Property(nameof(IAuditableEntity.CreatedOnUtc)).CurrentValue = utcNow;
                    entityEntry.Property(nameof(IAuditableEntity.CreatedBy)).CurrentValue = saveBy;
                }

                if (entityEntry.State == EntityState.Modified)
                {
                    entityEntry.Property(nameof(IAuditableEntity.ModifiedOnUtc)).CurrentValue = utcNow;
                    entityEntry.Property(nameof(IAuditableEntity.ModifiedBy)).CurrentValue = saveBy;
                }
            }
        }

        /// <summary>
        /// Updates the entities implementing <see cref="ISoftDeletableEntity"/> interface.
        /// </summary>
        /// <param name="utcNow">The current date and time in UTC format.</param>
        private void UpdateSoftDeletableEntities(Guid saveBy, DateTime utcNow)
        {
            foreach (EntityEntry<ISoftDeletableEntity> entityEntry in ChangeTracker.Entries<ISoftDeletableEntity>())
            {
                if (entityEntry.State != EntityState.Deleted)
                {
                    continue;
                }

                entityEntry.Property(nameof(ISoftDeletableEntity.DeletedOnUtc)).CurrentValue = utcNow;
                entityEntry.Property(nameof(IAuditableEntity.ModifiedBy)).CurrentValue = saveBy;

                entityEntry.Property(nameof(ISoftDeletableEntity.Deleted)).CurrentValue = true;

                entityEntry.State = EntityState.Modified;

                UpdateDeletedEntityEntryReferencesToUnchanged(entityEntry);
            }
        }

        /// <summary>
        /// Updates the specified entity entry's referenced entries in the deleted state to the modified state.
        /// This method is recursive.
        /// </summary>
        /// <param name="entityEntry">The entity entry.</param>
        private static void UpdateDeletedEntityEntryReferencesToUnchanged(EntityEntry entityEntry)
        {
            if (!entityEntry.References.Any())
            {
                return;
            }

#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
            foreach (ReferenceEntry referenceEntry in entityEntry.References.Where(r => r.TargetEntry != null && r.TargetEntry.State == EntityState.Deleted))
            {
                if (referenceEntry.TargetEntry == null)
                {
                    return;
                }
                referenceEntry.TargetEntry.State = EntityState.Unchanged;

                UpdateDeletedEntityEntryReferencesToUnchanged(referenceEntry.TargetEntry);
            }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
        }

        /// <summary>
        /// Publishes and then clears all the domain events that exist within the current transaction.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        private async Task PublishDomainEvents(CancellationToken cancellationToken)
        {
            List<EntityEntry<AggregateRoot>> aggregateRoots = ChangeTracker
                .Entries<AggregateRoot>()
                .Where(entityEntry => entityEntry.Entity.DomainEvents.Any())
                .ToList();

            List<IDomainEvent> domainEvents = aggregateRoots.SelectMany(entityEntry => entityEntry.Entity.DomainEvents).ToList();

            aggregateRoots.ForEach(entityEntry => entityEntry.Entity.ClearDomainEvents());

            IEnumerable<Task> tasks = domainEvents.Select(domainEvent => _mediator.Publish(domainEvent, cancellationToken));

            await Task.WhenAll(tasks);
        }

    }
}
