using System.Text.Json.Serialization;
using LittleViet.Data.Models;

namespace LittleViet.Data.ViewModels;

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
    public GenericAccountViewModel Account { get; set; }
}

public class GetListOrderParameters : BaseListQueryParameters
{
    public Guid? AccountId { get; set; }
    public IEnumerable<OrderType> OrderTypes { get; set; }
    public double? TotalPriceFrom { get; set; }
    public double? TotalPriceTo { get; set; }
    public IEnumerable<PaymentType> PaymentTypes { get; set; }
    public DateTime? PickupTimeTo { get; set; }
    public DateTime? PickupTimeFrom { get; set; }
}
