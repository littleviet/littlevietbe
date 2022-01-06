using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Models.Repositories;
using LittleViet.Data.ViewModels;
using LittleViet.Data.ServiceHelper;
using Microsoft.EntityFrameworkCore;


namespace LittleViet.Data.Domains;

public interface IReservationDomain
{
    Task<Reservation> CreateAsync(CreateReservationViewModel reservationVm);
}
internal class ReservationDomain : BaseDomain, IReservationDomain
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IMapper _mapper;
    public ReservationDomain(IUnitOfWork uow, IReservationRepository, ImMa mapper) : base(uow)
    {
        _reservationRepository = productTypeRepository;
        _mapper = mapper;
    }

    public async Task<ResponseViewModel> Create(CreateReservationViewModel createReservationViewModel)
    {
        try
        {
            var resevation = _mapper.Map<Reservation>(createReservationViewModel);

            resevation.Id = Guid.NewGuid();
            resevation.FirstName = createReservationViewModel.FirstName;
            resevation.LastName = createReservationViewModel.LastName;
            resevation.Email = createReservationViewModel.Email;
            resevation.FurtherRequest = createReservationViewModel.FurtherRequest;

            _reservationRepository.Create(Reservation);
            await _uow.SaveAsync();

            return new ResponseViewModel { Success = true, Message = "Create successful" };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }
}