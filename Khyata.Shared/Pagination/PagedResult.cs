namespace Khyata.Shared.Pagination
{
    public  class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];
        public int Page { get; init; }
        public int Limit { get; init; }
        public int TotalCount { get; init; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)Limit);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPrevPage => Page > 1;
    }

}
