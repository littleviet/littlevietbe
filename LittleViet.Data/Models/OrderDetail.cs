namespace LittleViet.Data.Models;

public class OrderDetail : AuditableEntity
{
    public Guid ServingId { get; set; }
    public Serving Serving { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
    public double Amount { get; set; }
    public double Price { get; set; }
}

