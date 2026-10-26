namespace WebApplication3.Contracts;

public class OrderListQuery : PaginationQuery
{
    public string? Search { get; init; }
    public string? Status { get; init; }
    public decimal? MinTotalAmount { get; init; }
    public decimal? MaxTotalAmount { get; init; }
    public DateTime? CreatedFromUtc { get; init; }
    public DateTime? CreatedToUtc { get; init; }
}
