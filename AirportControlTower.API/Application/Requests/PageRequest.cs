namespace AirportControlTower.API.Application.Requests
{
    public class PageRequest
    {
        public string? SortField { get; set; }
        public string SortDirection { get; set; } = "Desc";
        public string? SearchQuery { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
