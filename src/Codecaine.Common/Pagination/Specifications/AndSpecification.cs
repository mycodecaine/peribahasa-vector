using Codecaine.Common.Pagination.Interfaces;
using System.Linq.Expressions;

namespace Codecaine.Common.Pagination.Specifications
{
    public class AndSpecification<T> : ISpecification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left;
            _right = right;
        }

        public Expression<Func<T, bool>> Criteria => _left.Criteria.AndAlso(_right.Criteria);
    }

}
