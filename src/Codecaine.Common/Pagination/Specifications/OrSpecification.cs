using Codecaine.Common.Pagination.Interfaces;
using System.Linq.Expressions;

namespace Codecaine.Common.Pagination.Specifications
{
    public class OrSpecification<T> : ISpecification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left;
            _right = right;
        }

        public Expression<Func<T, bool>> Criteria => _left.Criteria.OrElse(_right.Criteria);
    }
}
