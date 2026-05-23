using Khyata.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Khyata.Infrastructure.Helpers
{
    public static class PaginationHelper
    {
        public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
        IQueryable<TEntity> query,
        int page,
        int limit,
        Func<TEntity, TDto> map) where TEntity : class
        {
            page = Math.Max(page, 1);
            limit = Math.Clamp(limit, 1, 100);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return new PagedResult<TDto>
            {
                Items = items.Select(map).ToList(),
                Page = page,
                Limit = limit,
                TotalCount = total
            };
        }

        public static string EncodeCursor(DateTime value) =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes(value.ToString("O")));

        public static DateTime? DecodeCursor(string? cursor)
        {
            if (string.IsNullOrWhiteSpace(cursor)) return null;
            try
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
                return DateTime.Parse(decoded, null,
                    System.Globalization.DateTimeStyles.RoundtripKind);
            }
            catch { return null; }
        }
    }
}
