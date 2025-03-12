
namespace AirportControlTower.Dashboard.Data
{
#nullable disable
    public class PaginatedEntities<TOptions, TData> where TOptions : PagingOptions
    {
        public required TOptions PagingOptions { get; set; } = default!;
        public required IEnumerable<TData> Data { get; set; } = [];
    }
    
    public class PaginatedEntities<TData>
    {
        public required PagingOptions PagingOptions { get; set; } = default!;
        public required IEnumerable<TData> Data { get; set; } = [];
    }

    public class PagingOptions
    {
        public string SortField { get; set; }
        public string SortDirection { get; set; } = "Desc";
        public string SearchString { get; set; }

        public int Page { get; set; } = 1;
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 15;

        public int[] PageSizes = [15, 30, 50, 100, 500, 1000];

        public int NumPages { get; set; } = 1;

        public PagingOptions()
        {

        }
    }
}
