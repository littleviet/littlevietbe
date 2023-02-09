using AutoMapper;
using LittleViet.Domain.Models;
using LittleViet.Domain.ViewModels;

namespace LittleViet.Domain.Domains.Coupon;

public class CouponMappers : Profile
{
    public CouponMappers()
    {

            CreateMap<Models.Coupon, CreateCouponViewModel>().ReverseMap();
            CreateMap<Models.Coupon, CouponDetailsViewModel>().ReverseMap();            
            CreateMap<CouponType, CreateCouponTypeViewModel>().ReverseMap();
    }
}