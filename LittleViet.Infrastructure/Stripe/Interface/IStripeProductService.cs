using LittleViet.Infrastructure.Stripe.Models;
using Stripe;

namespace LittleViet.Infrastructure.Stripe.Interface;

public interface IStripeProductService
{
    Task<Product> CreateProduct(CreateProductDto dto);
    Task<Product> UpdateProduct(UpdateProductDto dto);
    Task<Product> DeactivateProduct(string id);
}