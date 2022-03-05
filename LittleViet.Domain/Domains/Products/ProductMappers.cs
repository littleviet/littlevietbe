using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Stripe.Models;

namespace LittleViet.Data.Domains.Products;

public class ProductMappers : Profile
{
    public ProductMappers()
    {
            CreateMap<Product, CreateProductViewModel>().ReverseMap();
            CreateMap<Product, UpdateProductViewModel>().ReverseMap();
            CreateMap<Product, ProductsLandingPageViewModel>().ReverseMap();
            CreateMap<Product, ProductDetailsViewModel>().ReverseMap();
            
            CreateMap<ProductImage, CreateProductImageViewModel>().ReverseMap();
            CreateMap<ProductImage, GenericProductImageViewModel>().ReverseMap();

            CreateMap<UpdateProductViewModel, UpdateProductDto>().ReverseMap();
            CreateMap<CreateProductViewModel, CreateProductDto>().ReverseMap();
    }
}