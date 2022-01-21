namespace LittleViet.Infrastructure.Stripe;

public class StripeSettings
{
    public const string ConfigSection = "Stripe";
    
    public string SecretKey { get; set; }
    public string PublishableKey { get; set; }
    public string WebhookSecret { get; set; }
    public Payment Payment { get; set; }
}

public class Payment
{
    public const string OrderMetaDataKey = "orderId";
    public string PaymentType { get; set; } = "payment";
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
    public string BaseDomain { get; set; }
    public string Currency { get; set; }
}