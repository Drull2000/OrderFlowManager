using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;

namespace WebApplication3.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (!await dbContext.Orders.AnyAsync(cancellationToken))
        {
            var now = DateTime.UtcNow;
            var orders = new List<Order>
            {
                new() { Number = "ORD-1001", CustomerName = "Ivan Petrov", TotalAmount = 120.50m, Status = "New", CreatedAtUtc = now.AddDays(-12) },
                new() { Number = "ORD-1002", CustomerName = "Olga Ivanova", TotalAmount = 499.99m, Status = "Processing", CreatedAtUtc = now.AddDays(-10) },
                new() { Number = "ORD-1003", CustomerName = "Andrii Kovalenko", TotalAmount = 75.00m, Status = "Done", CreatedAtUtc = now.AddDays(-7) },
                new() { Number = "ORD-1004", CustomerName = "Svitlana Bondar", TotalAmount = 899.00m, Status = "Canceled", CreatedAtUtc = now.AddDays(-5) },
                new() { Number = "ORD-1005", CustomerName = "Pavlo Tkach", TotalAmount = 250.75m, Status = "Done", CreatedAtUtc = now.AddDays(-3) },
                new() { Number = "ORD-1006", CustomerName = "Maksym Horbunov", TotalAmount = 64.20m, Status = "New", CreatedAtUtc = now.AddDays(-1) }
            };

            await dbContext.Orders.AddRangeAsync(orders, cancellationToken);
        }

        if (!await dbContext.Users.AnyAsync(cancellationToken))
        {
            var now = DateTime.UtcNow;
            var users = new List<AppUser>
            {
                new() { UserName = "admin", Email = "admin@example.com", Role = "Admin", IsActive = true, CreatedAtUtc = now.AddDays(-30) },
                new() { UserName = "manager1", Email = "manager1@example.com", Role = "Manager", IsActive = true, CreatedAtUtc = now.AddDays(-20) },
                new() { UserName = "user1", Email = "user1@example.com", Role = "User", IsActive = true, CreatedAtUtc = now.AddDays(-15) },
                new() { UserName = "user2", Email = "user2@example.com", Role = "User", IsActive = false, CreatedAtUtc = now.AddDays(-8) },
                new() { UserName = "support", Email = "support@example.com", Role = "Support", IsActive = true, CreatedAtUtc = now.AddDays(-5) },
                new() { UserName = "auditor", Email = "auditor@example.com", Role = "Auditor", IsActive = false, CreatedAtUtc = now.AddDays(-2) }
            };

            await dbContext.Users.AddRangeAsync(users, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
