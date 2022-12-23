using System.Text.Json.Serialization;
using LittleViet.Domain.Models;

namespace LittleViet.Domain.ViewModels;

public class CreateOrderViewModel
{
    public OrderType OrderType { get; set; } = OrderType.TakeAway;
    public double TotalPrice { get; set; }
    [JsonIgnore]
    public Guid AccountId { get; set; }
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
    public GenericAccountViewModel Account { get; set; }
    public OrderType OrderType { get; set; } = OrderType.TakeAway;
    public double TotalPrice { get; set; }
    public PaymentType PaymentType { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime PickupTime { get; set; }
}

public class OrderDetailsViewModel
{
    public Guid Id { get; set; }
    public OrderType OrderType { get; set; } = OrderType.TakeAway;
    public double TotalPrice { get; set; }
    public PaymentType PaymentType { get; set; }
    public string StripeSessionUrl { get; set; }
    public DateTime PickupTime { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? CreatedDate { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid UpdatedBy { get; set; }
    public List<OrderDetailItemViewModel> OrderDetails { get; set; }
    public GenericAccountViewModel Account { get; set; }
}

public class GetListOrderParameters : BaseListQueryParameters<Order>
{
    public Guid? AccountId { get; set; }
    public IEnumerable<OrderType> OrderTypes { get; set; }
    public double? TotalPriceFrom { get; set; }
    public double? TotalPriceTo { get; set; }
    public string PhoneNumber { get; set; }
    public string FullName { get; set; }    
    public IEnumerable<OrderStatus> Statuses { get; set; }
    public IEnumerable<PaymentType> PaymentTypes { get; set; }
    public DateTime? PickupTimeTo { get; set; }
    public DateTime? PickupTimeFrom { get; set; }
}

#region OrderDetails
public class CreateOrderDetailViewModel
{
    public Guid ServingId { get; set; }
    public double Quantity { get; set; }
}

public class OrderDetailItemViewModel
{
    public Guid Id { get; set; }
    public Guid ServingId { get; set; }
    public string ServingName { get; set; }
    public double Quantity { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
}
#endregion
