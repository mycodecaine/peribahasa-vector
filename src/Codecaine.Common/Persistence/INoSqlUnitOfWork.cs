using Codecaine.Common.Domain;
using Codecaine.Common.Persistence.MongoDB.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Persistence
{
    public interface INoSqlUnitOfWork
    {
        Task StartTransactionAsync(Guid saveBy, CancellationToken cancellationToken = default);
        Task CommitAsync<TEntity>(TEntity entity) where TEntity : Entity;
     
    }
}
