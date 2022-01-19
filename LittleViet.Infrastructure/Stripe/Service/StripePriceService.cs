using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Models;
using Microsoft.Extensions.Options;
using Stripe;

namespace LittleViet.Infrastructure.Stripe.Service;

public class StripePriceService : BaseStripeService, IStripePriceService
{
    private readonly PriceService _priceService;

    public StripePriceService(IOptions<StripeSettings> stripeSettings, PriceService priceService) : base(
        stripeSettings)
    {
        _priceService = priceService ?? throw new ArgumentNullException(nameof(priceService));
    }

    public Task<Price> CreatePrice(CreatePriceDto dto)
    {
        var options = new PriceCreateOptions()
        {
            UnitAmount = dto.Price,
            Currency = dto.Currency,
            Product = dto.StripeProductId,
            
        };

        return _priceService.CreateAsync(options);
    }
    
    public async Task<Price> UpdatePrice(UpdatePriceDto dto)
    {
        await DeactivatePrice(dto.Id);
        
        var options = new PriceCreateOptions()
        {
            UnitAmount = dto.Amount,
            Currency = dto.Currency,
            Product = dto.ProductId,
        };

        return await _priceService.CreateAsync(options);
    }

    public Task<Price> DeactivatePrice(string id)
    {
        var options = new PriceUpdateOptions
        {
            Active = false
        };

        return _priceService.UpdateAsync(id, options);
    }
}