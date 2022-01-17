namespace LittleViet.Infrastructure.Stripe.Models;

public class UpdateProductDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> Images { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
    public string Url { get; set; }
}