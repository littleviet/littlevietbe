using LittleViet.Data.ServiceHelper;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleViet.Data.Models
{
    [Table("ProductType")]
    internal class ProductType: AuditableEntity
    {
        [Column("Name")]
        public string Name { get; set; }
    }
}
