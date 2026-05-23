using Khyata.Infrastructure.Data;
using Khyata.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace Khyata.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Runs every day and suspends any Active workspace whose NextSuspensionDate has passed.
    /// The 30-day clock starts when an admin activates (or reactivates) the workspace.
    /// Only an admin can reactivate a suspended workspace.
    /// </summary>
    public  class WorkspaceSuspensionService(
     IServiceScopeFactory scopeFactory,
     ILogger<WorkspaceSuspensionService> logger)
     : BackgroundService
    {
        private static readonly TimeSpan CheckInterval = TimeSpan.FromDays(1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("WorkspaceSuspensionService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SuspendExpiredWorkspacesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while checking workspace suspensions.");
                }

                await Task.Delay(CheckInterval, stoppingToken);
            }
        }

        private async Task SuspendExpiredWorkspacesAsync(CancellationToken ct)
        {
            using var scope = scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var expired = await _context.Workspaces
                .Where(w =>
                    w.Status == WorkspaceStatus.Active &&
                    w.NextSuspensionDate != null &&
                    w.NextSuspensionDate <= DateTime.UtcNow)
                .ToListAsync(ct);

            if (expired.Count == 0) return;

            logger.LogInformation("Suspending {Count} workspace(s).", expired.Count);

            foreach (var ws in expired)
            {
                ws.Status = WorkspaceStatus.Suspended;
                ws.NextSuspensionDate = null;

                //_context.AuditLogs.Add(new Domain.Entities.AuditLog
                //{
                //    Action = "WorkspaceAutoSuspended",
                //    EntityType = "Workspace",
                //    EntityId = ws.Id,
                //    Details = "Suspended automatically at end of month."
                //});

                logger.LogInformation("Workspace {Id} suspended.", ws.Id);
            }

            await _context.SaveChangesAsync(ct);
        }
    }
}
