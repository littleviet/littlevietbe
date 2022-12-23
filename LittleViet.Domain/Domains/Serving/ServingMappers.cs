using AutoMapper;
using LittleViet.Domain.ViewModels;
using LittleViet.Infrastructure.Stripe.Models;

namespace LittleViet.Domain.Domains.Serving;

public class ServingMappers : Profile
{
    public ServingMappers()
    {
            CreateMap<Models.Serving, CreateServingViewModel>().ReverseMap();
            CreateMap<Models.Serving, UpdateServingViewModel>().ReverseMap();
            CreateMap<Models.Serving, ServingViewDetailsModel>().ReverseMap();
            CreateMap<Models.Serving, GenericServingViewModel>().ReverseMap();

            CreateMap<CreateServingViewModel, CreatePriceDto>().ReverseMap();
    }
}