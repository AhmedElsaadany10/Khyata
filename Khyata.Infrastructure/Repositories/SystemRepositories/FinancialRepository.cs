using Khyata.Application.Common;
using Khyata.Application.DTOs.Order.Financial;
using Khyata.Application.Interfaces.IRepositories.ISystemRepositories;
using Khyata.Domain.Enums;
using Khyata.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Infrastructure.Repositories.SystemRepositories
{
    public class FinancialRepository : IFinancialRepository
    {
        private readonly AppDbContext _context;

        public FinancialRepository(AppDbContext context) => _context = context;
        public async Task<Result<FinancialReportResponseDto>> GetReportAsync(Guid workspaceId, FinancialReportQueryDto query)
        {
            var (from, to) = ResolveDateRange(query);

            var orders = await _context.Orders
                .Include(o => o.Payments)
                .Where(o => o.WorkspaceId == workspaceId
                         && o.CreatedAt >= from
                         && o.CreatedAt <= to)
                    .AsNoTracking()
                    .ToListAsync();

            var cancelled = orders.Where(o => o.Status == OrderStatus.Cancelled).ToList();
            var active = orders.Where(o => o.Status != OrderStatus.Cancelled).ToList();

            return Result<FinancialReportResponseDto>.Success(new FinancialReportResponseDto
            {
                TotalOrdersAmount = active.Sum(o => o.TotalPrice),

                TotalPaidAmount = active.Sum(o =>
                    o.Payments?.Sum(p => p.Amount) ?? 0),

                TotalRemainingAmount = active.Sum(o =>
                    o.TotalPrice - (o.Payments?.Sum(p => p.Amount) ?? 0)),

                TotalCancelledAmount = cancelled.Sum(o => o.TotalPrice),
                NumberOfOrders = active.Count,
                NumberOfCancelledOrders = cancelled.Count,
                ActiveRevenue = active.Sum(o => o.TotalPrice)
            });
        }
        private static (DateTime from, DateTime to) ResolveDateRange(FinancialReportQueryDto q)
        {
            var now = DateTime.UtcNow;
            return q.Period switch
            {
                "daily" => (now.Date, now.Date.AddDays(1).AddTicks(-1)),
                "weekly" => (now.AddDays(-(int)now.DayOfWeek).Date, now),
                "monthly" => (new DateTime(now.Year, now.Month, 1), now),
                "yearly" => (new DateTime(now.Year, 1, 1), now),
                "custom" => (q.From ?? now.AddMonths(-1), q.To ?? now),
                _ => (now.AddMonths(-1), now)
            };
        }
    }
}
