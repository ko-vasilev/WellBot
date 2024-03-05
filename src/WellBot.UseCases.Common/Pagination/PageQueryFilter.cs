using System.ComponentModel.DataAnnotations;

namespace WellBot.UseCases.Common.Pagination;

/// <summary>
/// Base class for filters that contains page, page size and sorting entries.
/// </summary>
public abstract class PageQueryFilter
{
    /// <summary>
    /// Page number to return. Starts with 1.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Required page size (amount of items returned at a time).
    /// </summary>
    [Range(1, 1000)]
    public int PageSize { get; set; } = 100;

    /// <summary>
    /// Specifies results ordering logic. Supporting fields for sorting: id, date.
    /// For sort descending use sort_field:desc. For example, id:desc.
    /// </summary>
    public virtual string OrderBy { get; set; } = string.Empty;
}
