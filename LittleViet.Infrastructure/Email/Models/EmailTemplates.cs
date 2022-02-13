namespace LittleViet.Infrastructure.Email.Models;

public static class EmailTemplates
{
    public class EmailTemplate
    {
        public string TemplateName { get; set; }
        public string SubjectName { get; set; }
    }
    
    public static readonly EmailTemplate ReservationSuccess = new ()
    {
        TemplateName = "reservation-success.html",
        SubjectName = "Reservation successful at Little Viet"
    };
    
    public static readonly EmailTemplate CouponPurchaseSuccess = new ()
    {
        TemplateName = "coupon-buying-success.html",
        SubjectName = "Coupon purchase at Little Viet"
    };
}