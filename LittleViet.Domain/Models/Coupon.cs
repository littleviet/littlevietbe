namespace LittleViet.Domain.Models;

public class Coupon : AuditableEntity
{
    public uint InitialQuantity { get; set; }
    public uint CurrentQuantity { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }    
    public string LastStripeSessionId { get; set; }
    public CouponStatus Status { get; set; }
    public virtual Account Account { get; set; }
    public Guid? AccountId { get; set; }
    public string CouponCode { get; set; }
    public Guid CouponTypeId { get; set; }
    public virtual CouponType CouponType { get; set; }
}