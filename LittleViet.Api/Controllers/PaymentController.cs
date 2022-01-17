using LittleViet.Data.Domains;
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
    private readonly IProductDomain _productDomain;
    private readonly StripeSettings _stripeSettings;
    public PaymentController(IProductDomain productDomain, IOptions<StripeSettings> stripeSettings)
    {
        _productDomain = productDomain ?? throw new ArgumentNullException(nameof(productDomain));
        _stripeSettings = stripeSettings.Value ?? throw new ArgumentNullException(nameof(stripeSettings));
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPost("")]
    public async Task<IActionResult> Create(CreateProductViewModel createProductViewModel)
    {
        try
        {
            var result = await _productDomain.Create(createProductViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
    
    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _stripeSettings.WebhookSecret
            );
            Console.WriteLine($"Webhook notification with type: {stripeEvent.Type} found for {stripeEvent.Id}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Something failed {e}");
            return BadRequest();
        }

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
            Console.WriteLine($"Session ID: {session.Id}");
            // Take some action based on session.
            
        }
        
        if (stripeEvent.Type == "checkout.session.expired")
        {
            var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
            Console.WriteLine($"Session ID: {session.Id}");
            // Take some action based on session.
            
        }

        return Ok();
    }
}

