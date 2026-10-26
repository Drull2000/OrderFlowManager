using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Contracts;

public class UserListQuery : PaginationQuery, IValidatableObject
{
    private static readonly HashSet<string> AllowedSortFields =
    [
        "id",
        "username",
        "email",
        "role",
        "isactive",
        "createdatutc"
    ];

    [StringLength(100, ErrorMessage = "Search не може бути довшим за 100 символів.")]
    public string? Search { get; init; }

    [StringLength(50, ErrorMessage = "Role не може бути довшим за 50 символів.")]
    public string? Role { get; init; }

    public bool? IsActive { get; init; }
    public DateTime? CreatedFromUtc { get; init; }
    public DateTime? CreatedToUtc { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Page < 1)
        {
            yield return new ValidationResult("Page має бути більше або дорівнювати 1.", [nameof(Page)]);
        }

        if (PageSize is < 1 or > 100)
        {
            yield return new ValidationResult("PageSize має бути в діапазоні від 1 до 100.", [nameof(PageSize)]);
        }

        if (!string.IsNullOrWhiteSpace(SortDir) &&
            !string.Equals(SortDir, "asc", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(SortDir, "desc", StringComparison.OrdinalIgnoreCase))
        {
            yield return new ValidationResult("SortDir може бути тільки 'asc' або 'desc'.", [nameof(SortDir)]);
        }

        if (!string.IsNullOrWhiteSpace(SortBy) && !AllowedSortFields.Contains(SortBy.Trim().ToLowerInvariant()))
        {
            yield return new ValidationResult(
                "SortBy містить недопустиме поле. Дозволено: id, userName, email, role, isActive, createdAtUtc.",
                [nameof(SortBy)]);
        }

        if (CreatedFromUtc.HasValue && CreatedToUtc.HasValue && CreatedFromUtc > CreatedToUtc)
        {
            yield return new ValidationResult(
                "CreatedFromUtc не може бути пізніше за CreatedToUtc.",
                [nameof(CreatedFromUtc), nameof(CreatedToUtc)]);
        }
    }
}
