using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains.Reservations;

public class ReservationMappers : Profile
{
    public ReservationMappers()
    {
            CreateMap<Reservation, ReservationDetailsViewModel>().ReverseMap();
            CreateMap<Reservation, UpdateReservationViewModel>().ReverseMap();
            CreateMap<Reservation, CreateReservationViewModel>().ReverseMap();
    }
}