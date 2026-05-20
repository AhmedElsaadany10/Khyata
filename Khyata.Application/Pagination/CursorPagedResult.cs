namespace Khyata.Shared.Pagination
{
    public  class CursorPagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];
        public string? NextCursor { get; init; }
        public int Limit { get; init; }
        public int Count => Items.Count;
    }
}
