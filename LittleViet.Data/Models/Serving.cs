using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.Models;

internal class Serving : AuditableEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public string Name { get; set; }
    public int NumberOfPeople { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
}

