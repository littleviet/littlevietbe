using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.Models;

internal class Coupon : AuditableEntity
{
    public double Amount { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public int Status { get; set; }
    public string CouponCode { get; set; }
}
