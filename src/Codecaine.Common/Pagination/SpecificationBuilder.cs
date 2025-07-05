using Codecaine.Common.Pagination.Interfaces;
using Codecaine.Common.Pagination.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Pagination
{
    public static class SpecificationBuilder
    {
        public static ISpecification<T>? Build<T>(IEnumerable<FilterCriterion> filters)
        {
            ISpecification<T>? spec = null;

            foreach (var filter in filters)
            {
                var current = ToSpecification<T>(filter);
#pragma warning disable S3358 // Ternary operators should not be nested
                spec = spec == null
                    ? current
                    : (filter.Logic == FilterLogic.And
                        ? new AndSpecification<T>(spec, current)
                        : new OrSpecification<T>(spec, current));
#pragma warning restore S3358 // Ternary operators should not be nested
            }

            return spec;
        }

        private static ISpecification<T> ToSpecification<T>(FilterCriterion criterion)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, criterion.Property);
            var valueType = property.Type;

            Expression? expression = null;

            foreach (var valueStr in criterion.Values)
            {
                object? value = Convert.ChangeType(valueStr, valueType);
                var constant = Expression.Constant(value, valueType);

                Expression currentExpr = criterion.Operator switch
                {
                    FilterOperator.Equals => Expression.Equal(property, constant),
                    FilterOperator.NotEquals => Expression.NotEqual(property, constant),
                    FilterOperator.GreaterThan => Expression.GreaterThan(property, constant),
                    FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, constant),
                    FilterOperator.LessThan => Expression.LessThan(property, constant),
                    FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(property, constant),
                    FilterOperator.Contains => Expression.Call(
                        Expression.Call(property, "ToString", Type.EmptyTypes),
                        typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                        Expression.Constant(valueStr)),
                    FilterOperator.In => Expression.Call(
                        Expression.Constant(criterion.Values),
                        typeof(List<string>).GetMethod("Contains", new[] { typeof(string) })!,
                        Expression.Call(property, "ToString", Type.EmptyTypes)),
                    _ => throw new NotImplementedException()
                };

                expression = expression == null ? currentExpr : Expression.OrElse(expression, currentExpr);
            }

            var lambda = Expression.Lambda<Func<T, bool>>(expression!, parameter);
            return new DynamicSpecification<T>(lambda);
        }
    }
}
