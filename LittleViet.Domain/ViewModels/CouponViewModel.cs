using System.Text.Json.Serialization;
using LittleViet.Data.Models;

namespace LittleViet.Data.ViewModels;

public class CreateCouponViewModel
{
    public uint Quantity { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    [JsonIgnore]
    public Guid? AccountId { get; set; }
    public Guid CouponTypeId { get; set; }
}
// \
// public uint InitialQuantity { get; set; }
// public uint CurrentQuantity { get; set; }
// public string Email { get; set; }
// public string PhoneNumber { get; set; }
// public CouponStatus Status { get; set; }
// public virtual Account Account { get; set; }
// public Guid? AccountId { get; set; }
// public string CouponCode { get; set; }
// public Guid CouponTypeId { get; set; }
// public virtual CouponType CouponType { get; set; }

public class UpdateCouponViewModel
{
    public Guid Id { get; set; }
    public double Amount { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public CouponStatus Status { get; set; }
}

public class UpdateCouponStatusViewModel
{
    public Guid Id { get; set; }
    public CouponStatus Status { get; set; }
}

public class CouponViewModel
{
    public Guid Id { get; set; }
    public double Amount { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public CouponStatus Status { get; set; }
    public string CouponCode { get; set; }
}

public class CouponDetailsViewModel
{
    public Guid Id { get; set; }
    public double Amount { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public CouponStatus Status { get; set; }
    public string StatusName { get; set; }
    public string CouponCode { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? CreatedDate { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid UpdatedBy { get; set; }
}

public class UseCouponViewModel
{
    public string couponCode { get; set; }
    public uint usage { get; set; }
}