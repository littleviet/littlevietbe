namespace LittleViet.Infrastructure.Email.Templates;

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
    
    
    public static EmailTemplate TakeAwayOrderPaymentSuccess =>
        new()
        {
            TemplateName = "order-paid.html",
            SubjectName = "Order payment successful at Little Viet",
            Keys = new HashSet<string>()
            {
                "name",
                "takeaway-time",
                "total-paid",
                "items",
                "order-id",
            }
        };
    
    public static EmailTemplate TakeAwayOrderPaymentSuccessAdmin =>
        new()
        {
            TemplateName = "order-paid-admin.html",
            SubjectName = "New order arrived",
            Keys = new HashSet<string>()
            {
                "name",
                "pickup-time",
                "total-paid",
                "payment-time",
                "items",
                "order-id",
            }
        };

    public static EmailTemplate CouponPurchaseSuccess => new()
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

    public static EmailTemplate CouponRedemptionSuccess => new()
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
}