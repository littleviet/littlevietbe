using AutoMapper;
using LittleViet.Domain.Models;
using LittleViet.Domain.ViewModels;

namespace LittleViet.Domain.Domains.Reservations;

public class ReservationMappers : Profile
{
    public ReservationMappers()
    {
            CreateMap<Reservation, ReservationDetailsViewModel>().ReverseMap();
            CreateMap<Reservation, UpdateReservationViewModel>().ReverseMap();
            CreateMap<Reservation, CreateReservationViewModel>().ReverseMap();
    }
}