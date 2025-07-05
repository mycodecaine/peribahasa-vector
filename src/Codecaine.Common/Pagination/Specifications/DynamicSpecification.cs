using Codecaine.Common.Pagination.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Pagination.Specifications
{
    public class DynamicSpecification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>> Criteria { get; }

        public DynamicSpecification(Expression<Func<T, bool>> expression)
        {
            Criteria = expression;
        }
    }
}
