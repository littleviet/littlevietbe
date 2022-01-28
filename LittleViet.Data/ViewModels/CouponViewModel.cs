using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.ViewModels;

public class CreateCouponViewModel
{
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

public class CouponViewModel
{
    public Guid Id { get; set; }
    public double Amount { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public CouponStatus Status { get; set; }
    public string StatusName { get; set; }
    public string CouponCode { get; set; }
}

public class CouponDetailsViewModel
{
    public Guid Id { get; set; }
     public double Amount { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public CouponStatus Status { get; set; }
    public string StatusName { get; set; }
    public string CouponCode { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? CreatedDate { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid UpdatedBy { get; set; }
}