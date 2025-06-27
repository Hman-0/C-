namespace ProductCatalogApi.DTOs
{
    public class PagedResultDto<T>
    {
        public List<T> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }

    public class StockReportDto
    {
        public decimal TotalValue { get; set; }
        public int TotalProducts { get; set; }
        public int TotalStock { get; set; }
        public List<CategoryStockDto> CategoryBreakdown { get; set; } = new();
    }

    public class CategoryStockDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal TotalValue { get; set; }
        public int ProductCount { get; set; }
        public int TotalStock { get; set; }
    }

    public class SellProductDto
    {
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }
}