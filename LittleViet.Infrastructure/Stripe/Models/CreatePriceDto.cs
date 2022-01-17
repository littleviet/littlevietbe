namespace LittleViet.Infrastructure.Stripe.Models;

public class CreatePriceDto
{
    public long? Amount { get; set; }
    public string Currency { get; set; }
    public string ProductId { get; set; }
}