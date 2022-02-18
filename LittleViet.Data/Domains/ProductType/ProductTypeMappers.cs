using AutoMapper;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains.ProductType;

public class ProductTypeMappers : Profile
{
    public ProductTypeMappers()
    {
            CreateMap<Models.ProductType, CreateProductTypeViewModel>().ReverseMap();
            CreateMap<Models.ProductType, UpdateProductTypeViewModel>().ReverseMap();
            CreateMap<Models.ProductType, ProductLandingPageViewModel>().ReverseMap();
            CreateMap<Models.ProductType, ProductTypeDetailsViewModel>().ReverseMap();
    }
}