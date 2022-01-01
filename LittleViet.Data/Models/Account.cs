using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.Models
{
    public class Account : AuditableEntity
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public RoleEnum AccountType { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
    }
}
