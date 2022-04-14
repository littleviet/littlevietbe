namespace LittleViet.Infrastructure.Email.Models;

public static class EmailTemplates
{
    public class EmailTemplate
    {
        public string TemplateName { get; init; }
        public string SubjectName { get; init; }
        public IEnumerable<string> Keys { get; init; }
    }

    public static EmailTemplate ReservationSuccess =>
        new()
        {
            TemplateName = "reservation-success.html",
            SubjectName = "Reservation successful at Little Viet",
            Keys = new HashSet<string>()
        {
            "name",
            "time",
            "no-of-people",
            "phone-number",
            "reservation-id",
        }
        };

    public static EmailTemplate CouponPurchaseSuccess =>
        new()
        {
            TemplateName = "coupon-buying-success.html",
            SubjectName = "Coupon purchase at Little Viet",
            Keys = new HashSet<string>()
        {
            "name",
            "time",
            "coupon-name",
            "phone-number",
            "usage-left",
            "coupon-id",
            "email",
            "coupon-code",
        }
        };

    public static EmailTemplate CouponRedemptionSuccess =>
        new()
        {
            TemplateName = "coupon-usage-success.html",
            SubjectName = "Coupon redeemed at Little Viet",
            Keys = new HashSet<string>()
        {
            "name",
            "usage",
            "total-value",
            "time",
            "coupon-name",
            "phone-number",
            "usage-left",
            "coupon-id",
            "email",
            "coupon-code",
        }
        };

    public static EmailTemplate NewReservation =>
        new()
        {
            TemplateName = "new-reservation.html",
            SubjectName = "New reservation at Little Viet",
            Keys = new HashSet<string>()
        {
            "name",
            "time",
            "no-of-people",
            "phone-number",
            "reservation-id",
        }
        };

    public static EmailTemplate TakeAwaySuccess =>
        new()
        {
            TemplateName = "reservation-success.html",
            SubjectName = "Reservation successful at Little Viet",
            Keys = new HashSet<string>()
        {
            "name",
            "time",
            "price",
            "payment-type",
            "phone-number",
        }
        };
}