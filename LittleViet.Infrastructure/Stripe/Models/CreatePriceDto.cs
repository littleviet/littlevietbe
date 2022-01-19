namespace LittleViet.Infrastructure.Stripe.Models;

public class CreatePriceDto
{
    public long? Price { get; set; }
    public string Currency { get; set; }
    public string StripeProductId { get; set; }
}