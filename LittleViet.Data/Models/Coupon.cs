using LittleViet.Data.ServiceHelper;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleViet.Data.Models
{
    [Table("Coupon")]
    internal class Coupon : AuditableEntity
    {
        [Column("Amount")]
        public double Amount { get; set; }
        [Column("Email")]
        public string Email { get; set; }
        [Column("PhoneNumber")]
        public string PhoneNumber { get; set; }
        [Column("Status")]
        public int Status { get; set; }
        [Column("CouponCode")]
        public string CouponCode { get; set; }
    }
}
