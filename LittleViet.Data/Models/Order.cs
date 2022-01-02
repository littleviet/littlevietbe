using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.Models;

internal class Order : AuditableEntity
{
    public OrderType OrderType { get; set; }
    public double TotalPrice { get; set; }
    public int PaymentType { get; set; }
    public DateTime PickupTime { get; set; }
}