using System.ComponentModel.DataAnnotations;

namespace LittleViet.Data.Models;

public enum RoleEnum
{
    ADMIN = 1,
    MANAGER = 2,
    AUTHORIZED = 3,
    UNAUTHORIZED = 4
}

public enum ProductStatus
{
    [Display(Name = "In stock")]
    InStock = 1,
    [Display(Name = "Out of stock")]
    OutOfStock = 2,
}

public enum ReservationStatus
{
    Reserved = 1,
    Cancelled = 2,
    Completed = 3,

}
public enum OrderType
{
    [Display(Name = "Eat-in")]
    EatIn = 1,
    [Display(Name = "Take away")]
    TakeAway = 2,
}

public enum CouponStatus
{
    Created = 1,
    Paid = 2,
    Used = 3,
}

public enum PaymentType
{
    Stripe = 1,
}

public enum OrderStatus
{
    Ordered = 1,
    Paid = 2,
    Cancelled = 3,
    Expired = 4,
    PickedUp = 5,
}

public class Role
{
    public const string ADMIN = "ADMIN";
    public const string AUTHORIZED = "AUTHORIZED";
    public const string UNAUTHORIZED = "UNAUTHORIZED";
    public const string MANAGER = "MANAGER";
}
