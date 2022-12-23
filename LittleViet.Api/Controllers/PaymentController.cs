using LittleViet.Data.Domains.Payment;
using LittleViet.Infrastructure.Stripe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using Stripe;

namespace LittleViet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : Controller
{
    private readonly StripeSettings _stripeSettings;
    private readonly IPaymentDomain _paymentDomain;
    public PaymentController(IOptions<StripeSettings> stripeSettings, IPaymentDomain paymentDomain)
    {
        _paymentDomain = paymentDomain ?? throw new ArgumentNullException(nameof(paymentDomain));
        _stripeSettings = stripeSettings.Value ?? throw new ArgumentNullException(nameof(stripeSettings));
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _stripeSettings.WebhookSecret
            );

            return stripeEvent.Type switch
            {
                Events.CheckoutSessionCompleted => Ok(
                    await _paymentDomain.HandleSuccessfulPayment(stripeEvent.Data.Object as Stripe.Checkout.Session)),
                Events.CheckoutSessionExpired => Ok(
                    await _paymentDomain.HandleExpiredPayment(stripeEvent.Data.Object as Stripe.Checkout.Session)),
                _ => BadRequest()
            };
        }
        catch (Exception e)
        {
            Log.Warning("Unable top process Stripe event with {exception} and {body}", e.ToString(), json);
            return BadRequest("Bad request from Stripe");
        }
    }
}
