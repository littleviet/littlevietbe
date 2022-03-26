using AutoMapper;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains.ProductType;

public class ProductTypeMappers : Profile
{
    public ProductTypeMappers()
    {
            CreateMap<Models.ProductType, CreateProductTypeViewModel>().ReverseMap();
            CreateMap<Models.ProductType, UpdateProductTypeViewModel>().ReverseMap();
            CreateMap<Models.ProductType, MenuProductTypeLandingPageViewModel>().ReverseMap();
            CreateMap<Models.ProductType, ProductTypeDetailsViewModel>().ReverseMap();
            CreateMap<Models.ProductType, GenericProductTypeViewModel>().ReverseMap();
    }
}