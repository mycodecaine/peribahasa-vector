using Codecaine.Common.Persistence.Dapper;
using Codecaine.Common.Persistence.Dapper.Interfaces;
using Codecaine.PeribahasaVector.Domain.Entities;
using Codecaine.PeribahasaVector.Domain.Repositories;

namespace Codecaine.PeribahasaVector.Infrastructure.DataAccess.Repositories
{
    public class PeribahasaRepository : DapperRepository<Peribahasa>, IPeribahasaRepository
    {
        public PeribahasaRepository(IDapperDbContext dbContext) : base(dbContext)
        {
        }
    }
}
