using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.Models;

public class Order : AuditableEntity
{
    public OrderType OrderType { get; set; }
    public double TotalPrice { get; set; }
    public PaymentType PaymentType { get; set; }
    public OrderStatus OrderStatus { get; set; } 
    public DateTime PickupTime { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; }
}