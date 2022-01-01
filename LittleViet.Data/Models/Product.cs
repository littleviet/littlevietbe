using LittleViet.Data.ServiceHelper;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleViet.Data.Models
{
    [Table("Product")]
    internal class Product : AuditableEntity
    {
        [Column("Name")]
        public string Name { get; set; }
        [Column("Description")]
        public string Description { get; set; }
        [Column("ProductType")]
        public int ProductType { get; set; }
        [Column("Status")]
        public int Status { get; set; }
    }
}
