using LittleViet.Data.ServiceHelper;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleViet.Data.Models
{
    internal class Order : AuditableEntity
    {
        [Column("OrderType")]
        public int OrderType { get; set; }
        [Column("TotalPrice")]
        public double TotalPrice { get; set; }
        [Column("Payment")]
        public int PaymentType { get; set; }
        [Column("Status")]
        public int  Status { get; set; }
        [Column("PickupTime")]
        public DateTime PickupTime { get; set; }
    }
}
