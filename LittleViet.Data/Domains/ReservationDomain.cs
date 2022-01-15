using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Models.Repositories;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Domains;

public interface IReservationDomain
{
    Task<ResponseViewModel> Create(CreateReservationViewModel reservationVm);
    Task<ResponseViewModel> Update(UpdateReservationViewModel reservationVm);
    Task<ResponseViewModel> Deactivate(Guid Id);
    Task<BaseListQueryResponseViewModel> Search(BaseSearchParameters parameters);
    Task<BaseListQueryResponseViewModel> GetListReservations(BaseListQueryParameters parameters);
    Task<ResponseViewModel> GetReservationById(Guid id);
}
internal class ReservationDomain : BaseDomain, IReservationDomain
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
            var date = DateTime.UtcNow;

            resevation.Id = Guid.NewGuid();
            resevation.CreatedBy = createReservationViewModel.AccountId;
            resevation.CreatedDate = date;
            resevation.UpdatedDate = date;
            resevation.UpdatedBy = createReservationViewModel.AccountId;
            resevation.IsDeleted = false;
            resevation.Status = ReservationStatus.Reserved;

            _reservationRepository.Add(resevation);
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

            if (existedReservation != null)
            {
                existedReservation.Firstname = updateReservationViewModel.FirstName;
                existedReservation.Lastname = updateReservationViewModel.LastName;
                existedReservation.Email = updateReservationViewModel.Email;
                existedReservation.NoOfPeople = updateReservationViewModel.NoOfPeople;
                existedReservation.BookingDate = updateReservationViewModel.BookingDate;
                existedReservation.FurtherRequest = updateReservationViewModel.FurtherRequest;
                existedReservation.Status = updateReservationViewModel.Status;
                existedReservation.PhoneNumber = updateReservationViewModel.PhoneNumber;
                existedReservation.UpdatedBy = updateReservationViewModel.UpdatedBy;
                existedReservation.UpdatedDate = DateTime.UtcNow;

                _reservationRepository.Modify(existedReservation);
                await _uow.SaveAsync();
                return new ResponseViewModel { Success = true, Message = "Update successful" };
            }

            return new ResponseViewModel { Success = false, Message = "This reservation does not exist" };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<ResponseViewModel> Deactivate(Guid Id)
    {
        try
        {
            var reservation = await _reservationRepository.GetById(Id);

            if (reservation != null)
            {
                _reservationRepository.Deactivate(reservation);
                await _uow.SaveAsync();

                return new ResponseViewModel { Success = true, Message = "Deactivate successful" };
            }

            return new ResponseViewModel { Success = false, Message = "This reservation does not exist" };
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
                .Where(p => p.Firstname.Contains(parameters.Keyword) || p.PhoneNumber.Contains(parameters.Keyword)
                || p.Email.Contains(parameters.Keyword) || p.Lastname.Contains(parameters.Keyword) || p.PhoneNumber.Contains(parameters.Keyword));

            return new BaseListQueryResponseViewModel
            {
                Payload = await reservations.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber).ToListAsync(),
                Success = true,
                Total = await reservations.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            return new BaseListQueryResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseListQueryResponseViewModel> GetListReservations(BaseListQueryParameters parameters)
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

    public async Task<ResponseViewModel> GetReservationById(Guid id)
    {
        try
        {
            var reservation = await _reservationRepository.GetById(id);

            if (reservation == null)
            {
                return new ResponseViewModel { Success = false, Message = "This reservation does not exist" };
            }

            return new ResponseViewModel { Success = true, Payload = reservation };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }
}

