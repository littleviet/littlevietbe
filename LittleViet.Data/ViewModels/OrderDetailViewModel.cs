namespace LittleViet.Data.ViewModels;

public class CreateOrderDetailViewModel
{
    public Guid ServingId { get; set; }
    public double Quantity { get; set; }
}

public class OrderDetailViewModel
{
    public Guid Id { get; set; }
    public Guid ServingId { get; set; }
    public double Quantity { get; set; }
}
