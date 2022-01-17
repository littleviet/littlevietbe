namespace LittleViet.Data.Models;

public class Serving : AuditableEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public string StripePriceId { get; set; }
    public string Name { get; set; }
    public int NumberOfPeople { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
}

