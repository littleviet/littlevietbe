using LittleViet.Infrastructure.Stripe.Interface;
using Microsoft.Extensions.Options;

namespace LittleViet.Infrastructure.Stripe.Service;

public class BaseStripeService : IBaseStripeService
{
    protected readonly StripeSettings _stripeSettings;

    protected BaseStripeService(IOptions<StripeSettings> stripeSettings)
    {
        _stripeSettings = stripeSettings.Value ?? throw new ArgumentNullException(nameof(stripeSettings));
    }
}