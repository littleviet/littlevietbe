using LittleViet.Data.Models;
using LittleViet.Data.ServiceHelper;
namespace LittleViet.Data.ViewModels;

public class CreateOrderViewModel
{
    public OrderType OrderType { get; set; } = OrderType.TakeAway;
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

public class OrderViewModel
{
    public Guid Id { get; set; }
    public AccountViewModel Account { get; set; }
    public OrderType OrderType { get; set; } = OrderType.TakeAway;
    public string OrderTypeName { get; set; }
    public double TotalPrice { get; set; }
    public PaymentType PaymentType { get; set; }
    public string PaymentTypeName { get; set; }
    public DateTime PickupTime { get; set; }
}

public class OrderDetailsViewModel
{
    public Guid Id { get; set; }
    public OrderType OrderType { get; set; } = OrderType.TakeAway;
    public string OrderTypeName { get; set; }
    public double TotalPrice { get; set; }
    public PaymentType PaymentType { get; set; }
    public string PaymentTypeName { get; set; }
    public DateTime PickupTime { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? CreatedDate { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid UpdatedBy { get; set; }
    public List<OrderDetailViewModel> OrderDetails { get; set; }
    public AccountViewModel Account { get; set; }
}
