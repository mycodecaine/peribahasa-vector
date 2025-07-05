using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Pagination
{
    public class FilterCriterion
    {
        public string Property { get; set; } = string.Empty;
        public FilterOperator Operator { get; set; } = FilterOperator.Equals;
        public List<string> Values { get; set; } = new();
        public FilterLogic Logic { get; set; } = FilterLogic.And;
    }
}
