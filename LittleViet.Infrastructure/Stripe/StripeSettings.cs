namespace LittleViet.Infrastructure.Stripe;

public class StripeSettings
{
    public const string ConfigSection = "Stripe";
    
    public string ApiKey { get; set; }
    public string WebhookSecret { get; set; }
    public Payment Payment { get; set; }
}

public class Payment
{
    public string PaymentType { get; set; } = "payment";
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
    public string BaseDomain { get; set; }
}