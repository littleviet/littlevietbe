using LittleViet.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

namespace LittleViet.Domain.Services.HealthChecks;

public class DatabaseCheck : IHealthCheck
{
    private readonly LittleVietContext _context;

    public DatabaseCheck(LittleVietContext context)
    {
        _context = context;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync("SELECT *  FROM \"__EFMigrationsHistory\"", cancellationToken);
            return HealthCheckResult.Healthy("Database healthy");
        }
        catch (Exception e)
        {
            Log.Error("Database failed with {exception}", e);
            return HealthCheckResult.Degraded($"Database degraded with error: {e.Message}");
        }
    }
}
