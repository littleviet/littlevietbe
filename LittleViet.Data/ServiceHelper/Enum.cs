using System.ComponentModel.DataAnnotations;

namespace LittleViet.Data.ServiceHelper;

public enum RoleEnum
{
    ADMIN = 1,
    MANAGER = 2,
    AUTHORIZED = 3,
    UNAUTHORIZED = 4
}

public enum ProductSatus
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
    Expired = 2,
    Used = 3,
}

public enum PaymentType
{
    Cash = 1,
    Coupon = 2,
    Wallet = 3,
}

public enum OrderStatus
{
    Ordered = 1,
    Paid = 2,
    Cancelled = 3,
    Expired = 4,
}