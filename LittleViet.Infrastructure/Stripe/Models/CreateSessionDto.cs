namespace LittleViet.Infrastructure.Stripe.Models;

public class CreateSessionDto
{
    public List<SessionItem> SessionItems { get; set; }
    
    public Dictionary<string, string> Metadata { get; set; }
}

public class SessionItem
{
    public string StripePriceId { get; set; }
    public long? Quantity { get; set; }
}