using LittleViet.Data.ServiceHelper;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleViet.Data.Models
{
    [Table("Reservation")]
    internal class Reservation : AuditableEntity
    {
        [Column("NoOfPeople")]
        public int NoOfPeople { get; set; }
        [Column("BookingDate")]
        public DateTime BookingDate { get; set; }
        [Column("AccountId")]
        public Guid AccountId { get; set; }
        [Column("Status")]
        public int Status { get; set; }
        [Column("PhoneNumber")]
        public string PhoneNumber { get; set; }
        [Column("Firstname")]
        public string Firstname { get; set; }
        [Column("Lastname")]
        public string Lastname { get; set; }
        [Column("Email")]
        public string Email { get; set; }
        [Column("FurtherRequest")]
        public string FurtherRequest { get; set; }
    }
}
