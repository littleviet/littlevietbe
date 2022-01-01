using LittleViet.Data.ServiceHelper;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleViet.Data.Models
{
    [Table("OrderDetail")]
    internal class OrderDetail : AuditableEntity
    {
        [Column("ServingId")]
        public Guid ServingId { get; set; }
        [Column("OrderId")]
        public Guid OrderId { get; set; }
        [Column("Amount")]
        public double Amount { get; set; }
        [Column("Price")]
        public double Price { get; set; }
    }
}
