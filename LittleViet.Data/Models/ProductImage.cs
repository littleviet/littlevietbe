using LittleViet.Data.ServiceHelper;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleViet.Data.Models
{
    [Table("ProductImage")]
    internal class ProductImage : AuditableEntity
    {
        [Column("ProductId")]
        public Guid ProductId { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("Url")]
        public string Url { get; set; }
        [Column("IsMain")]
        public bool IsMain { get; set; }
    }
}
