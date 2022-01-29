namespace LittleViet.Data.Models;

public class Coupon : AuditableEntity
{
    public double Amount { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public CouponStatus Status { get; set; }
    public string CouponCode { get; set; }
}
