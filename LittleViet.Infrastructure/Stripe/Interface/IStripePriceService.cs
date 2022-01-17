using LittleViet.Infrastructure.Stripe.Models;
using Stripe;

namespace LittleViet.Infrastructure.Stripe.Interface;

public interface IStripePriceService
{
    Task<Price> CreatePrice(CreatePriceDto dto);
    Task<Price> UpdatePrice(UpdatePriceDto dto);
    Task<Price> DeactivatePrice(string id);
}