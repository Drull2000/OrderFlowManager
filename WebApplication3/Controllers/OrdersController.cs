using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Contracts;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<Order>>> GetAll([FromQuery] OrderListQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Order> data = dbContext.Orders.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            data = data.Where(x =>
                x.Number.ToLower().Contains(search) ||
                x.CustomerName.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            var status = query.Status.Trim().ToLower();
            data = data.Where(x => x.Status.ToLower() == status);
        }

        if (query.MinTotalAmount.HasValue)
        {
            data = data.Where(x => x.TotalAmount >= query.MinTotalAmount.Value);
        }

        if (query.MaxTotalAmount.HasValue)
        {
            data = data.Where(x => x.TotalAmount <= query.MaxTotalAmount.Value);
        }

        if (query.CreatedFromUtc.HasValue)
        {
            data = data.Where(x => x.CreatedAtUtc >= query.CreatedFromUtc.Value);
        }

        if (query.CreatedToUtc.HasValue)
        {
            data = data.Where(x => x.CreatedAtUtc <= query.CreatedToUtc.Value);
        }

        data = ApplySorting(data, query.SortBy, query.SortDir);

        var totalCount = await data.CountAsync(cancellationToken);
        var items = await data
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return Ok(new PagedResult<Order>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        });
    }

    private static IQueryable<Order> ApplySorting(IQueryable<Order> source, string? sortBy, string sortDir)
    {
        var isDesc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "number" => isDesc ? source.OrderByDescending(x => x.Number) : source.OrderBy(x => x.Number),
            "customername" => isDesc ? source.OrderByDescending(x => x.CustomerName) : source.OrderBy(x => x.CustomerName),
            "totalamount" => isDesc ? source.OrderByDescending(x => x.TotalAmount) : source.OrderBy(x => x.TotalAmount),
            "status" => isDesc ? source.OrderByDescending(x => x.Status) : source.OrderBy(x => x.Status),
            "createdatutc" => isDesc ? source.OrderByDescending(x => x.CreatedAtUtc) : source.OrderBy(x => x.CreatedAtUtc),
            _ => isDesc ? source.OrderByDescending(x => x.Id) : source.OrderBy(x => x.Id)
        };
    }
}
