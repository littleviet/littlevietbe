using LittleViet.Data.Models;

namespace LittleViet.Data.ViewModels;

public class CreateCouponTypeViewModel
{
    public double Value { get; set; }
    public string Name { get; set; }
}

public class GetCouponTypeViewModel
{
    public Guid Id { get; set; }
    public decimal Value { get; set; }
    public string Name { get; set; }
    public string Currency { get; set; }
}