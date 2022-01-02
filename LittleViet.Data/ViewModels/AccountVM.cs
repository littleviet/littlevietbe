using LittleViet.Data.Models;
using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.ViewModels;

    public class AccountVM
    {
        public string Email { get; set; }
        public RoleEnum AccountType { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid UpdatedBy { get; set; }
        public string Token { get; set; }
    }

    public class LoginVM
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class CreateAccountVM
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
        public Guid CreatedBy { get; set; }
    }
    public class UpdateAccountVM
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public RoleEnum AccountType { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public Guid UpdatedBy { get; set; }
    }
    
    public class UpdatePasswordVM
    {
        public Guid Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

