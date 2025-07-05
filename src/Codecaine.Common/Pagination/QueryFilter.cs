namespace Codecaine.Common.Pagination
{
    public  class QueryFilter
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
        public List<FilterCriterion> Filters { get; set; } = new();
        public bool OrLogic { get; set; } = false;

        public QueryFilter()
        {
        }

        public QueryFilter(int page, int pageSize, string? sortBy, bool sortDescending, List<FilterCriterion> filters, bool orLogic)
        {
            Page = page;
            PageSize = pageSize;
            SortBy = sortBy;
            SortDescending = sortDescending;
            Filters = filters;
            OrLogic = orLogic;
        }
    }
}
