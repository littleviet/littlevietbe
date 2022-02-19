using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains.Order;

public class OrderMappers : Profile
{
    public OrderMappers()
    {
            CreateMap<Models.Order, CreateOrderViewModel>().ReverseMap();
            CreateMap<Models.Order, UpdateOrderViewModel>().ReverseMap();
            CreateMap<Models.Order, OrderDetailsViewModel>().ReverseMap();
            CreateMap<OrderDetail, CreateOrderDetailViewModel>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailItemViewModel>().ReverseMap();
    }
}