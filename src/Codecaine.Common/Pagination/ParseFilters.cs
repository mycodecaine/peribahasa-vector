using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Pagination
{
    public class ParseFilters
    {
        public List<FilterCriterion> ParseFilterCriterion(List<string> rawFilters, bool orLogic)
        {
            var result = new List<FilterCriterion>();

            foreach (var raw in rawFilters)
            {
                var parts = raw.Split('|');
                if (parts.Length < 3) continue;

                var property = parts[0];
                var op = Enum.TryParse<FilterOperator>(parts[1], true, out var filterOp) ? filterOp : FilterOperator.Equals;
                var values = parts[2].Split(',').ToList();

                result.Add(new FilterCriterion
                {
                    Property = property,
                    Operator = op,
                    Values = values,
                    Logic = orLogic ? FilterLogic.Or : FilterLogic.And
                });
            }

            return result;
        }

    }
}
