namespace LittleViet.Infrastructure.Stripe.Models;

public class CreateSessionDto
{
    public List<SessionItem> SessionItems { get; set; }
}

public class SessionItem
{
    public string Price { get; set; }
    public long? Quantity { get; set; }
}