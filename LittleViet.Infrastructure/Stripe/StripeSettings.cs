namespace LittleViet.Infrastructure.Stripe;

public class StripeSettings
{
    public string ApiKey { get; set; }
    public Payment Payment { get; set; }
}

public class Payment
{
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
    public string BaseDomain { get; set; }
}