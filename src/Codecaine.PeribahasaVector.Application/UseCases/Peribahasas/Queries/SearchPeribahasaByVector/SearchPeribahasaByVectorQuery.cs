using Codecaine.Common.CQRS.Queries;
using Codecaine.Common.Primitives.Maybe;
using Codecaine.PeribahasaVector.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.PeribahasaVector.Application.UseCases.Peribahasas.Queries.SearchPeribahasaByVector
{
    public record SearchPeribahasaByVectorQuery(string Content):IQuery<Maybe<List<PeribahasaViewModel>>>;
    
}
