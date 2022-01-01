using LittleViet.Data.ServiceHelper;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleViet.Data.Models
{
    [Table("Account")]
    public class Account : AuditableEntity
    {
        [Column("Email")]
        public string Email { get; set; }
        [Column("Password")]
        public string Password { get; set; }
        [Column("AccountType")]
        public int AccountType { get; set; }
        [Column("Firstname")]
        public string Firstname { get; set; }
        [Column("Lastname")]
        public string Lastname { get; set; }
        [Column("Address")]
        public string Address { get; set; }
        [Column("PostalCode")]
        public string PostalCode { get; set; }
        [Column("PhoneNumber1")]
        public string PhoneNumber1 { get; set; }
        [Column("PhoneNumber2")]
        public string PhoneNumber2 { get; set; }
    }
}
