using LittleViet.Data.ServiceHelper;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleViet.Data.Models
{
    [Table("Serving")]
    internal class Serving : AuditableEntity
    {
        [Column("ProductId")]
        public Guid ProductId { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("NumberOfPeople")]
        public int NumberOfPeople { get; set; }
        [Column("Description")]
        public string Description { get; set; }
        [Column("Status")]
        public int Status { get; set; }
        [Column("Price")]
        public double Price { get; set; }
    }
}
