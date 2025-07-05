using Codecaine.Common.Abstractions;
using Codecaine.Common.AiServices.Interfaces;
using Codecaine.Common.Domain;
using Codecaine.Common.Domain.Interfaces;
using Codecaine.Common.Exceptions;
using Codecaine.Common.Persistence.Dapper.Interfaces;
using Codecaine.Common.Primitives.Maybe;
using Dapper;
using MediatR;
using System.Data;

namespace Codecaine.Common.Persistence.Dapper
{
    /// <summary>
    /// Notes about https://chatgpt.com/share/684802e3-e814-8007-9e9d-861fa1ef822a
    /// </summary>
    public class DapperDbContext : IDapperDbContext, IDapperUnitOfWork, IDisposable
    {
        private readonly IDbConnection _connection;
        private IDbTransaction? _transaction;
        private readonly IDateTime _dateTime;
        private readonly IMediator _mediator;
        private bool _disposed;
        private readonly IEmbeddingService _embeddingService;
        private Guid SaveBy { get; set; }

        public DapperDbContext(IDbConnection connection, IDateTime dateTime, IMediator mediator, IEmbeddingService embeddingService)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _embeddingService = embeddingService ?? throw new ArgumentNullException(nameof(embeddingService));
        }

        public IDbConnection Connection => _connection;
        public IDbTransaction? Transaction => _transaction;   

        public async Task<Maybe<TEntity>> GetBydIdAsync<TEntity>(Guid id) where TEntity : Entity
        {
            var tableName = GetTableName(typeof(TEntity));
            string sql = $"SELECT * FROM {tableName} WHERE Id = @Id;";
            var parameters = new DynamicParameters();
            parameters.Add("@Id" , id);

            return await _connection.QueryFirstOrDefaultAsync<TEntity>(sql, parameters);
        }

        public async Task Insert<TEntity>(TEntity entity) where TEntity : Entity
        {
            var tableName = GetTableName(typeof(TEntity));
            SetAuditProperties(entity, SaveBy);
            await UpdateVector(entity); // Ensure vector is updated before insertion

            var (sql, parameters) = GenerateInsert(tableName, entity);
            if (_transaction != null)
            {
              await  _connection.ExecuteAsync(sql, parameters, _transaction);
            }
            else
            {
              await  _connection.ExecuteAsync(sql, parameters);
            }
           
        }
        public async Task Update<TEntity>(TEntity entity) where TEntity : Entity
        {
            var tableName = GetTableName(typeof(TEntity));
            SetAuditProperties(entity, SaveBy);
            await UpdateVector(entity);
            var (sql, parameters) = GenerateUpdate(tableName, entity);
            if (_transaction != null)
            {
              await  _connection.ExecuteAsync(sql, parameters, _transaction);
            }
            else
            {
               await _connection.ExecuteAsync(sql, parameters);
            }
        }

        public async Task InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities) where TEntity : Entity
        {
            foreach (var entity in entities)
            {
              await  Insert(entity);
            }
        }

        public async Task Remove<TEntity>(TEntity entity) where TEntity : Entity
        {
            var isMarkedAsDeleted = IsMarkAsDeleted(entity);
            if (isMarkedAsDeleted)
            {
              await  Update(entity);
                return;

            }
            var tableName = typeof(TEntity).Name;
            var (sql, parameters) = GenerateDelete(tableName, entity.Id);
            if (_transaction != null)
            {
              await  _connection.ExecuteAsync(sql, parameters, _transaction);
            }
            else
            {
               await _connection.ExecuteAsync(sql, parameters);
            }
        }
        public Task StartTransactionAsync(Guid saveBy, CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
                _transaction = _connection.BeginTransaction();

            SaveBy = saveBy;
            return Task.CompletedTask;
        }
        public async Task CommitAsync<TEntity>(TEntity entity) where TEntity : Entity
        {
            try
            {
                if (_transaction == null)
                {
                    throw new CommonLibraryException( new Primitives.Errors.Error("TransactionHasNotStarted", "Transaction has not been started. Call StartTransactionAsync first."));
                }
                _transaction?.Commit();
                _transaction = null;
                await PublishDomainEvent(entity);
            }
            catch (Exception ex)
            {
                RollbackTransaction();
               
                throw new CommonLibraryException(new Primitives.Errors.Error("FailedCommitTransaction", $"Failed to commit transaction : { ex.Message }"));
            }
        }

        public async Task<IEnumerable<(string Content, double Similarity)>> SearchContentVectorAsync<TEntity>(string input, int topK = 5) where TEntity : Entity
        {
            var queryEmbedding = await _embeddingService.GetVectorAsync(input);
            var tableName = GetTableName(typeof(TEntity));

            var embeddingString = string.Join(",", queryEmbedding);
            var sqlVector = $@"
                        SELECT content,  (embedding <#> @Embedding::vector) AS similarity
                        FROM    ""{tableName}""
                        ORDER BY embedding <#> @Embedding::vector
                        LIMIT @TopK;";

            return await _connection.QueryAsync<(string Content, double Similarity)>(sqlVector, new
            {
                Embedding = $"[{embeddingString}]",
                TopK = topK
            });
        }

        public async Task<IEnumerable<(Guid id, double Similarity)>> SearchIdByVectorAsync<TEntity>(string input, int topK = 5) where TEntity : Entity
        {
            var queryEmbedding = await _embeddingService.GetVectorAsync(input);
            var tableName = GetTableName(typeof(TEntity));
            var embeddingString = string.Join(",", queryEmbedding);
            var sqlVector = $@"
                        SELECT id,  (embedding <#> @Embedding::vector) AS similarity
                        FROM    ""{tableName}"" 
                        ORDER BY embedding <#> @Embedding::vector
                        LIMIT @TopK;";

            return await _connection.QueryAsync<(Guid Id, double Similarity)>(sqlVector, new
            {
                Embedding = $"[{embeddingString}]",
                TopK = topK
            });
        }

        public async Task<IEnumerable<(TEntity entity, double Similarity)>> SearchEntityByVectorAsync<TEntity>(string input, int topK = 5) where TEntity : Entity
        {
            var data = await SearchIdByVectorAsync<TEntity>(input, topK);
            var tasks = data.Select(async item =>
            {
                var entity = await GetBydIdAsync<TEntity>(item.id);
                return (entity.Value, item.Similarity);
            });

            return await Task.WhenAll(tasks);
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
        private bool IsMarkAsDeleted<TEntity>(TEntity entity) where TEntity : Entity
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
        private static void SetProperty<TEntity>(TEntity target, string propertyName, object value) where TEntity : Entity
        {
            var property = target.GetType().GetProperty(propertyName);
            property?.SetValue(target, value);
        }

        private static (string Sql, DynamicParameters Parameters) GenerateInsert<T>(string tableName, T entity)
        {
            var props = typeof(T).GetProperties().Where(p => p.CanRead && p.Name != "DomainEvents").ToList();
            var columnNames = string.Join(", ", props.Select(p => p.Name));
            var paramNames = string.Join(", ", props.Select(p => "@" + p.Name));

            var sql = $"INSERT INTO {tableName} ({columnNames}) VALUES ({paramNames});";

            var parameters = new DynamicParameters();
            foreach (var prop in props)
            {
                if (prop.Name == "Embedding" )
                {
                    var value = prop.GetValue(entity);

                    // Handle null and support both List<float> and float[]
                    float[] data = value switch
                    {
                        List<float> list => list.ToArray(),
                        float[] array => array,
                        IEnumerable<float> enumerable => enumerable.ToArray(),
                        null => Array.Empty<float>(),
                        _ => throw new InvalidCastException($"Property {prop.Name} is not a valid float vector.")
                    };

                    parameters.Add("@" + prop.Name, data);
                }
                else
                {
                    parameters.Add("@" + prop.Name, prop.GetValue(entity));
                }
            }

            return (sql, parameters);
        }

        private static (string Sql, DynamicParameters Parameters) GenerateUpdate<T>(string tableName, T entity, string keyName = "Id")
        {
            var props = typeof(T).GetProperties().Where(p => p.CanRead && p.Name != keyName && p.Name != "DomainEvents").ToList();
            var setClause = string.Join(", ", props.Select(p => $"{p.Name} = @{p.Name}"));

            var sql = $"UPDATE {tableName} SET {setClause} WHERE {keyName} = @{keyName};";

            var parameters = new DynamicParameters();
            foreach (var prop in props)
            {
                if (prop.Name == "Embedding")
                {
                    var value = prop.GetValue(entity);

                    // Handle null and support both List<float> and float[]
                    float[] data = value switch
                    {
                        List<float> list => list.ToArray(),
                        float[] array => array,
                        IEnumerable<float> enumerable => enumerable.ToArray(),
                        null => Array.Empty<float>(),
                        _ => throw new InvalidCastException($"Property {prop.Name} is not a valid float vector.")
                    };

                    parameters.Add("@" + prop.Name, data);
                }
                else
                {
                    parameters.Add("@" + prop.Name, prop.GetValue(entity));
                }
            }

            var keyProp = typeof(T).GetProperty(keyName);
            parameters.Add("@" + keyName, keyProp?.GetValue(entity));

            return (sql, parameters);
        }

        private static (string Sql, DynamicParameters Parameters) GenerateDelete(string tableName, Guid keyValue, string keyName = "Id")
        {
            var sql = $"DELETE FROM {tableName} WHERE {keyName} = @{keyName};";
            var parameters = new DynamicParameters();
            parameters.Add("@" + keyName, keyValue);
            return (sql, parameters);
        }

        private void RollbackTransaction()
        {
            _transaction?.Rollback();
            _transaction = null;
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

        private async Task UpdateVector<TEntity>(TEntity entity) where TEntity : Entity
        {
            if(entity is AggregateVectorRoot aggregateRoot)
            {
              await  aggregateRoot.UpdateEmbeddingAsync(_embeddingService);  
            }
        }

        private static string GetTableName(Type type)
        {
            // Use custom attribute or convention-based logic here
            return type.Name.ToLower();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _connection.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

       

        ~DapperDbContext()
        {
            Dispose(false);
        }
    }


}
