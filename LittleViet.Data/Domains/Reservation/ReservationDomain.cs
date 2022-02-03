using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Email.Interface;
using LittleViet.Infrastructure.Email.Models;
using LittleViet.Infrastructure.EntityFramework;
using LittleViet.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Domains.Reservation;

public interface IReservationDomain
{
    Task<ResponseViewModel> Create(CreateReservationViewModel reservationVm);
    Task<ResponseViewModel> Update(UpdateReservationViewModel reservationVm);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListResponseViewModel> Search(BaseSearchParameters parameters);
    Task<BaseListResponseViewModel> GetListReservations(BaseListQueryParameters parameters);
    Task<ResponseViewModel> GetReservationById(Guid id);
}

internal class ReservationDomain : BaseDomain, IReservationDomain
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly ITemplateService _templateService;

    public ReservationDomain(IUnitOfWork uow, IReservationRepository reservationRepository, IMapper mapper,
        ITemplateService templateService, IEmailService emailService) : base(uow)
    {
        _reservationRepository =
            reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    public async Task<ResponseViewModel> Create(CreateReservationViewModel createReservationViewModel)
    {
        try
        {
            var reservation = _mapper.Map<Models.Reservation>(createReservationViewModel);

            reservation.Id = Guid.NewGuid();
            reservation.Status = ReservationStatus.Reserved;

            await using var transaction = _uow.BeginTransation();

            try
            {
                _reservationRepository.Add(reservation);
                await _uow.SaveAsync();

                var template = await _templateService.GetTemplateEmail(EmailTemplates.ReservationSuccess);

                var body = template
                    .Replace("{name}", createReservationViewModel.FirstName)
                    .Replace("{time}", createReservationViewModel.BookingDate.ToString("hh:mm:ss MM/dd/yyyy"))
                    .Replace("{no-of-people}", createReservationViewModel.NoOfPeople.ToString())
                    .Replace("{phone-number}", createReservationViewModel.PhoneNumber)
                    .Replace("{reservation-id}", reservation.Id.ToString());

                await _emailService.SendEmailAsync(
                    body: body,
                    toName: createReservationViewModel.FirstName,
                    toAddress: createReservationViewModel.Email,
                    subject: EmailTemplates.ReservationSuccess.SubjectName
                );

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return new ResponseViewModel {Success = true, Message = "Create successful"};
        }
        catch (Exception e)
        {
            return new ResponseViewModel {Success = false, Message = e.Message};
        }
    }

    public async Task<ResponseViewModel> Update(UpdateReservationViewModel updateReservationViewModel)
    {
        try
        {
            var existedReservation = await _reservationRepository.GetById(updateReservationViewModel.Id);

            if (existedReservation == null)
                return new ResponseViewModel {Success = false, Message = "This reservation does not exist"};

            existedReservation.Firstname = updateReservationViewModel.FirstName;
            existedReservation.Lastname = updateReservationViewModel.LastName;
            existedReservation.Email = updateReservationViewModel.Email;
            existedReservation.AccountId = updateReservationViewModel.AccountId;
            existedReservation.NoOfPeople = updateReservationViewModel.NoOfPeople;
            existedReservation.BookingDate = updateReservationViewModel.BookingDate;
            existedReservation.FurtherRequest = updateReservationViewModel.FurtherRequest;
            existedReservation.Status = updateReservationViewModel.Status;
            existedReservation.PhoneNumber = updateReservationViewModel.PhoneNumber;

            _reservationRepository.Modify(existedReservation);
            await _uow.SaveAsync();
            return new ResponseViewModel {Success = true, Message = "Update successful"};
        }
        catch (Exception e)
        {
            return new ResponseViewModel {Success = false, Message = e.Message};
        }
    }

    public async Task<ResponseViewModel> Deactivate(Guid id)
    {
        try
        {
            var reservation = await _reservationRepository.GetById(id);

            if (reservation == null)
                return new ResponseViewModel {Success = false, Message = "This reservation does not exist"};

            _reservationRepository.Deactivate(reservation);
            await _uow.SaveAsync();

            return new ResponseViewModel {Success = true, Message = "Deactivate successful"};
        }
        catch (Exception e)
        {
            return new ResponseViewModel {Success = false, Message = e.Message};
        }
    }

    public async Task<BaseListResponseViewModel> Search(BaseSearchParameters parameters)
    {
        try
        {
            var reservations = _reservationRepository.DbSet().AsNoTracking()
                .Where(p => p.Firstname.Contains(parameters.Keyword) || p.PhoneNumber.Contains(parameters.Keyword)
                                                                     || p.Email.Contains(parameters.Keyword) ||
                                                                     p.Lastname.Contains(parameters.Keyword) ||
                                                                     p.PhoneNumber.Contains(parameters.Keyword));

            return new BaseListResponseViewModel
            {
                Payload = await reservations.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                    .Select(q => new ReservationViewModel()
                    {
                        BookingDate = q.BookingDate,
                        Email = q.Email,
                        FirstName = q.Firstname,
                        FurtherRequest = q.FurtherRequest,
                        Id = q.Id,
                        LastName = q.Lastname,
                        NoOfPeople = q.NoOfPeople,
                        PhoneNumber = q.PhoneNumber,
                        Status = q.Status,
                        StatusName = q.Status.ToString()
                    }).ToListAsync(),
                Success = true,
                Total = await reservations.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            return new BaseListResponseViewModel {Success = false, Message = e.Message};
        }
    }

    public async Task<BaseListResponseViewModel> GetListReservations(BaseListQueryParameters parameters)
    {
        try
        {
            var reservation = _reservationRepository.DbSet().AsNoTracking();

            return new BaseListResponseViewModel
            {
                Payload = await reservation
                    .Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                    .ApplySort(parameters.OrderBy)
                    .Select(q => new ReservationViewModel()
                    {
                        BookingDate = q.BookingDate,
                        Email = q.Email,
                        FirstName = q.Firstname,
                        FurtherRequest = q.FurtherRequest,
                        Id = q.Id,
                        LastName = q.Lastname,
                        NoOfPeople = q.NoOfPeople,
                        PhoneNumber = q.PhoneNumber,
                        Status = q.Status,
                        StatusName = q.Status.ToString()
                    })
                    .ToListAsync(),
                Success = true,
                Total = await reservation.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            return new BaseListResponseViewModel {Success = false, Message = e.Message};
        }
    }

    public async Task<ResponseViewModel> GetReservationById(Guid id)
    {
        try
        {
            var reservation = await _reservationRepository.GetById(id);
            var reservationDetails = _mapper.Map<ReservationDetailsViewModel>(reservation);

            reservationDetails.StatusName = reservation.Status.ToString();

            return reservation == null
                ? new ResponseViewModel {Success = false, Message = "This reservation does not exist"}
                : new ResponseViewModel {Success = true, Payload = reservationDetails};
        }
        catch (Exception e)
        {
            return new ResponseViewModel {Success = false, Message = e.Message};
        }
    }
}