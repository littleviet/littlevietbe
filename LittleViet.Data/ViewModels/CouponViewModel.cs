using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.ViewModels;

public class CreateCouponViewModel
{
    public Guid CreatedBy { get; set; }
    public double Amount { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}

public class UpdateCouponViewModel
{
    public Guid Id { get; set; }
    public Guid UpdatedBy { get; set; }
    public double Amount { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public CouponStatus Status { get; set; }
}

public class UpdateCouponStatusViewModel
{
    public Guid Id { get; set; }
    public CouponStatus Status { get; set; }
}
