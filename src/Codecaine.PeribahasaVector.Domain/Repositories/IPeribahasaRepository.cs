using Codecaine.Common.Persistence.Dapper.Interfaces;
using Codecaine.PeribahasaVector.Domain.Entities;

namespace Codecaine.PeribahasaVector.Domain.Repositories
{
    /// <summary>
    /// Peribahasa repository interface for Dapper operations.
    /// Dapper is used for high-performance data access in .NET applications.
    /// Vector database operations are handled by this repository.
    /// </summary>
    public interface IPeribahasaRepository : IDapperRepository<Peribahasa>
    {
    }
}
