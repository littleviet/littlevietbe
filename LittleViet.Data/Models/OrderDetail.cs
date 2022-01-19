namespace LittleViet.Data.Models;

public class OrderDetail : AuditableEntity
{
    public Guid ServingId { get; set; }
    public virtual Serving Serving { get; set; }
    public Guid OrderId { get; set; }
    public virtual Order Order { get; set; }
    public long Quantity { get; set; }
    public double Price { get; set; }
}

