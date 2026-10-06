namespace Identity.Application.Common.Models;

public class PagedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => Page * PageSize < TotalCount;
    public bool HasPreviousPage => Page > 1;

    public PagedList(IEnumerable<T> items, int count, int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = count;
        Items = items.ToList();
    }
}
