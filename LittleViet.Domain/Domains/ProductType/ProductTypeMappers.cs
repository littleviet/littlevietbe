using AutoMapper;
using LittleViet.Domain.ViewModels;

namespace LittleViet.Domain.Domains.ProductType;

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