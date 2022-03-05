using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Stripe;

namespace LittleViet.Infrastructure.Stripe.Service;

public class StripeProductService : BaseStripeService, IStripeProductService
{
    private readonly ProductService _productService;

    public StripeProductService(IOptions<StripeSettings> stripeSettings, ProductService productService) : base(
        stripeSettings)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    public Task<Product> CreateProduct(CreateProductDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        
        var options = new ProductCreateOptions
        {
            Name = dto.Name,
            Images = dto.Images,
            Description = dto.Description,
            Metadata = dto.Metadata,
            Url = dto.Url
        };

        return _productService.CreateAsync(options);
    }

    public Task<Product> UpdateProduct(UpdateProductDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var options = new ProductUpdateOptions
        {
            Name = dto.Name,
            Images = dto.Images,
            Description = dto.Description,
            Metadata = dto.Metadata,
            Url = dto.Url
        };

        return _productService.UpdateAsync(dto.Id, options);
    }
    
    public Task<Product> DeactivateProduct(string id)
    {
        var options = new ProductUpdateOptions
        {
            Active = false
        };

        return _productService.UpdateAsync(id, options);
    }
}