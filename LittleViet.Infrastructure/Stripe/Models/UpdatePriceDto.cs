namespace LittleViet.Infrastructure.Stripe.Models;

public class UpdatePriceDto
{
    public string Id { get; set; }
    public long? Amount { get; set; }
    public string Currency { get; set; }
    public string ProductId { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
}