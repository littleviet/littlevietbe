using LittleViet.Data.ServiceHelper;
namespace LittleViet.Data.ViewModels;

public class CreateOrderViewModel
{
    public Guid CreatedBy { get; set; }
    public OrderType OrderType { get; set; }
    public double TotalPrice { get; set; }
    public PaymentType PaymentType { get; set; }
    public DateTime PickupTime { get; set; }
    public List<CreateOrderDetailViewModel> OrderDetails { get; set; }
}

public class UpdateOrderViewModel
{
    public Guid Id { get; set; }
    public Guid UpdatedBy { get; set; }
    public OrderType OrderType { get; set; }
    public double TotalPrice { get; set; }
    public PaymentType PaymentType { get; set; }
    public DateTime PickupTime { get; set; }
}

