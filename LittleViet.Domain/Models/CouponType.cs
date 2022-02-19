namespace LittleViet.Data.Models;

public class CouponType : AuditableEntity
{
    public double Value { get; set; }
    public string Name { get; set; }
    public string StripePriceId { get; set; }
}
