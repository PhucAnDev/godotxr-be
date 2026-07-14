namespace GodotXR.Domain.Shared
{
    public sealed class PagedResult<TItem>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public IReadOnlyList<TItem> Items { get; set; } = Array.Empty<TItem>();
    }
}
