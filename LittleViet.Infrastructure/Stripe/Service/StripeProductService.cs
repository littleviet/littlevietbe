using LittleViet.Infrastructure.Stripe.Interface;
using Stripe;

namespace LittleViet.Infrastructure.Stripe.Service;

public class StripeProductService : IStripeProductService
{
    public StripeProductService(StripeSettings stripeSettings)
    {
        _stripeSettings = stripeSettings;
        StripeConfiguration.ApiKey = "";
    }

    private readonly ProductService _productService;
    private readonly StripeSettings _stripeSettings;
    private RequestOptions getDefaultRequestOptions() =>
        new RequestOptions()
        {
        };

    public async Task CreateProduct()
    {
        var options = new ProductCreateOptions();
        var service = new ProductService();

        var product = await service.CreateAsync(options);
        
        
    }
    
    public async Task UpdateProduct()
    {
        var options = new ProductUpdateOptions();
        var service = new ProductService();

        // var product = await service.UpdateAsync(options);
    }
}