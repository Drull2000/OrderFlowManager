namespace WebApplication3.Contracts;

public class PaginationQuery
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    public int Page { get; init; } = 1;

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value <= 0 ? 10 : Math.Min(value, MaxPageSize);
    }

    public string? SortBy { get; init; }
    public string SortDir { get; init; } = "asc";
}
