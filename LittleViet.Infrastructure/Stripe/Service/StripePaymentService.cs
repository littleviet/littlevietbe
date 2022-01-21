using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace LittleViet.Infrastructure.Stripe.Service;

public class StripePaymentService : BaseStripeService, IStripePaymentService
{
    private readonly SessionService _sessionService;

    public StripePaymentService(IOptions<StripeSettings> stripeSettings, SessionService sessionService) : base(
        stripeSettings)
    {
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    }

    public Task<Session> CreateCheckoutSession(CreateSessionDto dto)
    {
        var options = new SessionCreateOptions
        {
            LineItems = dto.SessionItems
                .Select(i => new SessionLineItemOptions
                {
                    Price = i.StripePriceId,
                    Quantity = i.Quantity,
                }).ToList(),
            Mode = _stripeSettings.Payment.PaymentType,
            Metadata = dto.Metadata,
            SuccessUrl = $"{_stripeSettings.Payment.BaseDomain}/{_stripeSettings.Payment.SuccessUrl}",
            CancelUrl = $"{_stripeSettings.Payment.BaseDomain}/{_stripeSettings.Payment.CancelUrl}",
        };
        return _sessionService.CreateAsync(options);
    }
}