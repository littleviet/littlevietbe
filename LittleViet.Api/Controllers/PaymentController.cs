using System.Security.Claims;
using LittleViet.Data.Domains;
using LittleViet.Data.Domains.Payment;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Stripe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
            
            switch (stripeEvent.Type)
            {
                case Events.CheckoutSessionCompleted:
                    var sessionSuccess = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    return Ok(await _paymentDomain.HandleSuccessfulPayment(sessionSuccess));
                case Events.CheckoutSessionExpired:
                    var sessionExpired = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    return Ok(await _paymentDomain.HandleSuccessfulPayment(sessionExpired));
                default:
                    return BadRequest();
            }
        }
        catch (Exception e)
        {
            return BadRequest("Bad request from Stripe");
        }

    }
}

