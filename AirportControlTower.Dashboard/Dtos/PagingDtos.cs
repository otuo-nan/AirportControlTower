namespace AirportControlTower.Dashboard.Dtos
{

#nullable disable
    public class PagingOptionsDto
    {
        public string CurrentSortField { get; set; } 
        public string CurrentSortDirection { get; set; } = "desc";
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class PageMetaData
    {
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 20;
        public int TotalCount { get; set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public void Reset()
        {
            CurrentPage = 1;
            TotalCount = 0;
            TotalPages = 0;
        }
    }

    public class PagingLink(int page, bool enabled, string text)
    {
        public string Text { get; set; } = text;
        public int Page { get; set; } = page;
        public bool Enabled { get; set; } = enabled;
        public bool Active { get; set; }
    }
}
