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
    Task<ResponseViewModel> Create(CreateReservationViewModel reservationVm);
    Task<ResponseViewModel> Update(UpdateReservationViewModel reservationVm);
    Task<ResponseViewModel> Deactivate(Guid Id);
    Task<BaseListQueryResponseViewModel> Search(BaseSearchParameters parameters);
    Task<BaseListQueryResponseViewModel> GetListReservation(BaseListQueryParameters parameters);
}
public class ReservationDomain : BaseDomain, IReservationDomain
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IMapper _mapper;
    public ReservationDomain(IUnitOfWork uow, IReservationRepository reservationRepository, IMapper mapper) : base(uow)
    {
        _reservationRepository = reservationRepository;
        _mapper = mapper;
    }

    public async Task<ResponseViewModel> Create(CreateReservationViewModel createReservationViewModel)
    {
        try
        {
            var resevation = _mapper.Map<Reservation>(createReservationViewModel);

            resevation.Id = Guid.NewGuid();
            resevation.Firstname = createReservationViewModel.FirstName;
            resevation.Lastname = createReservationViewModel.LastName;
            resevation.Email = createReservationViewModel.Email;
            resevation.FurtherRequest = createReservationViewModel.FurtherRequest;
            resevation.BookingDate = createReservationViewModel.BookingDate;
            resevation.NoOfPeople = createReservationViewModel.NoOfPeople;
            resevation.CreatedBy = createReservationViewModel.CreatedBy;

            _reservationRepository.Create(resevation);
            await _uow.SaveAsync();

            return new ResponseViewModel { Success = true, Message = "Create successful" };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<ResponseViewModel> Update(UpdateReservationViewModel updateReservationViewModel)
    {
        try
        {
            var existedReservation = await _reservationRepository.GetById(updateReservationViewModel.Id);

            if(existedReservation != null)
            {
                existedReservation.Firstname = updateReservationViewModel.FirstName;
                existedReservation.Lastname = updateReservationViewModel.LastName;
                existedReservation.Email = updateReservationViewModel.Email;
                existedReservation.NoOfPeople = updateReservationViewModel.NoOfPeople;
                existedReservation.BookingDate = updateReservationViewModel.BookingDate;
                existedReservation.FurtherRequest = updateReservationViewModel.FurtherRequest;
                existedReservation.UpdatedBy = updateReservationViewModel.UpdatedBy;
            }
            

            
            _reservationRepository.Modify(existedReservation);
            await _uow.SaveAsync();

            return new ResponseViewModel { Success = true, Message = "Update successful" };
        } catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message= e.Message };
        }
    }

    public async Task<ResponseViewModel> Deactivate(Guid Id)
    {
        try
        {
            var reservation =  await _reservationRepository.GetById(Id);
            _reservationRepository.DeactivateReservation(reservation);
            await _uow.SaveAsync();

            return new ResponseViewModel { Success = true, Message = "Deactivate successful" };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseListQueryResponseViewModel> Search(BaseSearchParameters parameters)
    {
        try
        {
            var reservations = _reservationRepository.DbSet().AsNoTracking()
                .Where(p => p.Firstname.Contains(parameters.Keyword) || p.PhoneNumber.Contains(parameters.Keyword) || p.Email.Contains(parameters.Keyword));

            return new BaseListQueryResponseViewModel
            {
                Payload = await reservations.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber).ToListAsync(),
                Success = true,
                Total = await reservations.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        } catch (Exception e)
        {
            return new BaseListQueryResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseListQueryResponseViewModel> GetListReservation(BaseListQueryParameters parameters)
    {
        try
        {
            var reservation = _reservationRepository.DbSet().AsNoTracking();

            return new BaseListQueryResponseViewModel
            {
                Payload = await reservation.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber).ToListAsync(),
                Success = true,
                Total = await reservation.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            return new BaseListQueryResponseViewModel { Success = false, Message = e.Message };
        }
    }



}