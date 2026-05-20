namespace Khyata.Shared.Pagination
{
    public  class PaginationQuery
    {
        public int Page { get; init; } = 1;
        public int Limit { get; init; } = 20;
        public string? Cursor { get; init; }

        public int SafeLimit => Math.Clamp(Limit, 1, 100);
        public int SafePage => Math.Max(Page, 1);
    }
}
