using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains.Coupon;

public class CouponMappers : Profile
{
    public CouponMappers()
    {

            CreateMap<Models.Coupon, CreateCouponViewModel>().ReverseMap();
            CreateMap<Models.Coupon, CouponDetailsViewModel>().ReverseMap();            
            CreateMap<CouponType, CreateCouponTypeViewModel>().ReverseMap();
    }
}