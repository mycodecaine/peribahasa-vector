using Codecaine.Common.Domain;
using System.Linq.Expressions;

namespace Codecaine.Common.Persistence
{
    /// <summary>
    /// Represents a base class for defining specifications that encapsulate query logic for entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public abstract class Specification<TEntity>
       where TEntity : Entity
    {
        /// <summary>
        /// Gets the page number for pagination.
        /// </summary>
        public int? PageNumber { get; protected set; }

        /// <summary>
        /// Gets the page size for pagination.
        /// </summary>
        public int? PageSize { get; protected set; }

        /// <summary>
        /// Converts the specification to an expression predicate.
        /// </summary>
        /// <returns>The expression predicate.</returns>
        public abstract Expression<Func<TEntity, bool>> ToExpression();

        /// <summary>
        /// Gets the expression used for ordering results in ascending order.
        /// </summary>
        public Expression<Func<TEntity, object>>? OrderBy { get; protected set; }

        /// <summary>
        /// Gets the expression used for ordering results in descending order.
        /// </summary>
        public Expression<Func<TEntity, object>>? OrderByDescending { get; protected set; }

        /// <summary>
        /// Gets the property name used for ordering results in ascending order.
        /// </summary>
        public string? OrderByString { get; protected set; }

        /// <summary>
        /// Gets the property name used for ordering results in descending order.
        /// </summary>
        public string? OrderByDescendingString { get; protected set; }

        /// <summary>
        /// Gets the list of expressions used for including related entities in the query.
        /// </summary>
        public List<Expression<Func<TEntity, object>>> Includes { get; } = new();

        /// <summary>
        /// Adds an include expression to the specification for related entities.
        /// </summary>
        /// <param name="includeExpression">The include expression.</param>
        protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        /// <summary>
        /// Applies pagination to the specification.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        protected void ApplyPaging(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        /// <summary>
        /// Gets a value indicating whether the query results should be distinct.
        /// </summary>
        public bool IsDistinct { get; protected set; }

        /// <summary>
        /// Applies distinct filtering to the specification.
        /// </summary>
        protected void ApplyDistinct() => IsDistinct = true;

        /// <summary>
        /// Applies ordering to the specification using an ascending order expression.
        /// </summary>
        /// <param name="orderByExpression">The order by expression.</param>
        protected void ApplyOrderBy(Expression<Func<TEntity, object>> orderByExpression) =>
            OrderBy = orderByExpression;

        /// <summary>
        /// Applies ordering to the specification using an ascending order property name.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        protected void ApplyOrderBy(string propertyName) =>
            OrderByString = propertyName;

        /// <summary>
        /// Applies ordering to the specification using a descending order property name.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        protected void ApplyOrderByDescending(string propertyName) =>
            OrderByDescendingString = propertyName;

        /// <summary>
        /// Applies ordering to the specification using a descending order expression.
        /// </summary>
        /// <param name="orderByDescExpression">The order by descending expression.</param>
        protected void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderByDescExpression) =>
            OrderByDescending = orderByDescExpression;

        /// <summary>
        /// Checks if the specified entity satisfies this specification.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>True if the entity satisfies the specification, otherwise false.</returns>
        public bool IsSatisfiedBy(TEntity entity) => ToExpression().Compile()(entity);

        /// <summary>
        /// Implicitly converts the specification to an expression predicate.
        /// </summary>
        /// <param name="specification">The specification.</param>
        public static implicit operator Expression<Func<TEntity, bool>>(Specification<TEntity> specification) =>
            specification.ToExpression();
    }
}
