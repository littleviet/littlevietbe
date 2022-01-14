using LittleViet.Infrastructure.Stripe.Interface;
using Microsoft.Extensions.Options;

namespace LittleViet.Infrastructure.Stripe.Service;

public class BaseStripeService : IBaseStripeService
{
    private readonly StripeSettings _stripeSettings;

    public BaseStripeService(IOptions<StripeSettings> stripeSettings)
    {
        _stripeSettings = stripeSettings.Value ?? throw new ArgumentNullException(nameof(stripeSettings));
    }
}