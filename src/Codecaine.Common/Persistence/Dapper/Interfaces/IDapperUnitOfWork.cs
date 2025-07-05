using Codecaine.Common.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Persistence.Dapper.Interfaces
{
    public interface IDapperUnitOfWork
    {
        Task StartTransactionAsync(Guid saveBy, CancellationToken cancellationToken = default);
        Task CommitAsync<TEntity>(TEntity entity) where TEntity : Entity;
    }
}
