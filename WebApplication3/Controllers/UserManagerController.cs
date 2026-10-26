using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Contracts;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/usermanager")]
public class UserManagerController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<AppUser>>> GetAll([FromQuery] UserListQuery query, CancellationToken cancellationToken)
    {
        IQueryable<AppUser> data = dbContext.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            data = data.Where(x =>
                x.UserName.ToLower().Contains(search) ||
                x.Email.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.Role))
        {
            var role = query.Role.Trim().ToLower();
            data = data.Where(x => x.Role.ToLower() == role);
        }

        if (query.IsActive.HasValue)
        {
            data = data.Where(x => x.IsActive == query.IsActive.Value);
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

        return Ok(new PagedResult<AppUser>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        });
    }

    private static IQueryable<AppUser> ApplySorting(IQueryable<AppUser> source, string? sortBy, string sortDir)
    {
        var isDesc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "username" => isDesc ? source.OrderByDescending(x => x.UserName) : source.OrderBy(x => x.UserName),
            "email" => isDesc ? source.OrderByDescending(x => x.Email) : source.OrderBy(x => x.Email),
            "role" => isDesc ? source.OrderByDescending(x => x.Role) : source.OrderBy(x => x.Role),
            "isactive" => isDesc ? source.OrderByDescending(x => x.IsActive) : source.OrderBy(x => x.IsActive),
            "createdatutc" => isDesc ? source.OrderByDescending(x => x.CreatedAtUtc) : source.OrderBy(x => x.CreatedAtUtc),
            _ => isDesc ? source.OrderByDescending(x => x.Id) : source.OrderBy(x => x.Id)
        };
    }
}
