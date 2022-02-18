using AutoMapper;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Stripe.Models;

namespace LittleViet.Data.Domains.Serving;

public class ServingMappers : Profile
{
    public ServingMappers()
    {
            CreateMap<Models.Serving, CreateServingViewModel>().ReverseMap();
            CreateMap<Models.Serving, UpdateServingViewModel>().ReverseMap();
            CreateMap<Models.Serving, ServingViewDetailsModel>().ReverseMap();

            CreateMap<CreateServingViewModel, CreatePriceDto>().ReverseMap();
    }
}