using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Stripe.Models;

namespace LittleViet.Data.Domains.Products;

public class ProductMappers : Profile
{
    public ProductMappers()
    {
            CreateMap<CreateProductViewModel, Product>()
                .ForMember(x => x.ProductImages, y => y.Ignore())
                .ReverseMap();
            CreateMap<Product, UpdateProductViewModel>().ReverseMap();
            CreateMap<Product, MenuProductItemLandingPageViewModel>().ReverseMap();
            CreateMap<Product, ProductDetailsViewModel>().ReverseMap();            
            CreateMap<Product, PackagedProductViewModel>().ReverseMap();

            
            CreateMap<ProductImage, GenericProductImageViewModel>().ReverseMap();

            CreateMap<UpdateProductViewModel, UpdateProductDto>().ReverseMap();
            CreateMap<Product, UpdateProductDto>().ReverseMap();
            CreateMap<CreateProductViewModel, CreateProductDto>().ReverseMap();
    }
}