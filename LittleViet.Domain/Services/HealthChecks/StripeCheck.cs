using LittleViet.Domain.Models;
using LittleViet.Infrastructure.Stripe;
using LittleViet.Infrastructure.Stripe.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Serilog;

namespace LittleViet.Domain.Services.HealthChecks;

public class StripeCheck : IHealthCheck
{
    private readonly IStripeProductService _productService;
    private readonly StripeSettings _options;

    public StripeCheck(IStripeProductService productService, IOptions<StripeSettings> options)
    {
        _productService = productService;
        _options = options.Value;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _productService.GetProduct(_options.CouponProductId);
            return HealthCheckResult.Healthy("Stripe connection healthy");
        }
        catch (Exception e)
        {
            Log.Error("Stripe connection failed with {exception}", e);
            return HealthCheckResult.Degraded($"Stripe connection degraded with error: {e.Message}");
        }
    }
}