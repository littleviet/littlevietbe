namespace LittleViet.Domain.Models;

public class CouponType : AuditableEntity
{
    public decimal Value { get; set; }
    public string Name { get; set; }
    public string StripePriceId { get; set; }
}
