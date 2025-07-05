using Codecaine.Common.Abstractions;
using Codecaine.Common.Domain;
using Codecaine.Common.Domain.Interfaces;
using Codecaine.Common.Persistence.MongoDB.Interfaces;
using Codecaine.Common.Primitives.Maybe;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Codecaine.Common.Persistence.MongoDB
{
    public class MongoDbContext : IMongoDbContext 
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoClient _client;
        private readonly IDateTime _dateTime;
        private readonly IMediator _mediator;
        private readonly bool _isStandAlone;
        
        private Guid SaveBy { get;  set; }
        private IClientSessionHandle ClientSessionHandle { get; set; }




        public MongoDbContext(IOptions<MongoDbSetting> mongoSettings, IDateTime dateTime, IMediator mediator)
        {
            var settings = mongoSettings.Value;
            _client = new MongoClient(settings.ConnectionString);
            _database = _client.GetDatabase(settings.DatabaseName);
            _isStandAlone = settings.IsStandalone;  
            _dateTime = dateTime;
            _mediator = mediator;
        }

        public async Task<Maybe<TEntity>> GetBydIdAsync<TEntity>(Guid id) where TEntity : Entity
        {
            var data = await GetCollection<TEntity>(typeof(TEntity).Name).Find(x => x.Id == id).FirstOrDefaultAsync();
            return data == null ? Maybe<TEntity>.None : Maybe<TEntity>.From(data);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }

        public IQueryable<T> AsQueryable<T>(string name)
        {
            return _database.GetCollection<T>(name).AsQueryable();
        }

        public void Insert<TEntity>(TEntity entity) where TEntity : Entity
        {

            SetAuditProperties(entity, SaveBy);

            if (_isStandAlone)
            {
                GetCollection<TEntity>(typeof(TEntity).Name).InsertOne(entity);
                return;
            }

            GetCollection<TEntity>(typeof(TEntity).Name).InsertOne(ClientSessionHandle, entity);

        }        

        public void InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities) where TEntity : Entity
        {
            foreach (var entity in entities)
            {
                Insert(entity);
            }
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : Entity
        {
           
            var isMarkedAsDeleted = IsMarkAsDeleted( entity);
            if (isMarkedAsDeleted)
            {
                Update(entity);
                return;
            }

            if (_isStandAlone)
            {
                GetCollection<TEntity>(typeof(TEntity).Name)
                    .DeleteOne(x => x.Id == entity.Id);
                return;
            }

            GetCollection<TEntity>(typeof(TEntity).Name)
                .DeleteOne(ClientSessionHandle, x => x.Id == entity.Id);
        }       

        public async Task<IClientSessionHandle> StartSessionAsync()
        {
            return await _client.StartSessionAsync();
        }       

        public void Update<TEntity>(TEntity entity) where TEntity : Entity
        {
            SetAuditProperties( entity, SaveBy);

            if (_isStandAlone)
            {
                GetCollection<TEntity>(typeof(TEntity).Name)
                    .ReplaceOne(x => x.Id == entity.Id, entity);
                return;
            }

            GetCollection<TEntity>(typeof(TEntity).Name)
                .ReplaceOne(ClientSessionHandle, x => x.Id == entity.Id, entity);
        }

        public  async Task StartTransactionAsync(Guid saveBy, CancellationToken cancellationToken = default)
        {
            SaveBy = saveBy;

            if (!_isStandAlone)
            {
                ClientSessionHandle = await _client.StartSessionAsync();
            }         
          
        }

        public async Task CommitAsync<TEntity>(TEntity entity) where TEntity : Entity
        {
            try
            {
                if (!_isStandAlone)
                {
                    if (ClientSessionHandle is null)
                    {
                        throw new InvalidOperationException("Session handle is null. Start a session before committing.");
                    }
                    ClientSessionHandle.StartTransaction();
                    await ClientSessionHandle.CommitTransactionAsync();
                }

                await PublishDomainEvent(entity);
            }
            catch (Exception ex)
            {
                
                throw new InvalidOperationException("Failed to commit transaction", ex);
            }

        }    



        private void SetAuditProperties<TEntity>(TEntity entity, Guid currentUserId) where TEntity : Entity
        {
            if (entity is IAuditableEntity auditable)
            {
                var now = _dateTime.UtcNow;

                if (auditable.CreatedOnUtc == default)
                {
                    // Set Created fields if not already set
                    SetProperty(entity, nameof(auditable.CreatedOnUtc), now);
                    SetProperty(entity, nameof(auditable.CreatedBy), currentUserId);
                }

                // Always set Modified fields
                SetProperty(entity, nameof(auditable.ModifiedOnUtc), now);
                SetProperty(entity, nameof(auditable.ModifiedBy), currentUserId);
            }

        }


        private bool IsMarkAsDeleted<TEntity>( TEntity entity) where TEntity : Entity
        {
            if (entity is ISoftDeletableEntity softDeletable)
            {
                var now = _dateTime.UtcNow;

                SetProperty(entity, nameof(softDeletable.Deleted), true);
                SetProperty(entity, nameof(softDeletable.DeletedOnUtc), now);
                return true;
            }
            return false;
        }
        private async Task PublishDomainEvent<TEntity>(TEntity entity) where TEntity : Entity
        {
            if (entity is AggregateRoot aggregateRoot)
            {
                foreach (var domainEvent in aggregateRoot.DomainEvents)
                {
                    await _mediator.Publish(domainEvent);
                }

                aggregateRoot.ClearDomainEvents(); // Optional: clear after publishing
            }
        }
        private static void SetProperty<TEntity>(TEntity target, string propertyName, object value) where TEntity : Entity
        {
            var property = target.GetType().GetProperty(propertyName);
            property?.SetValue(target, value);
        }
    }
}
