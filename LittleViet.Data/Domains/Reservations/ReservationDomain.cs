using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Email.Interface;
using LittleViet.Infrastructure.Email.Models;
using LittleViet.Infrastructure.Mvc;
using static LittleViet.Infrastructure.Mvc.SqlHelper;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Domains.Reservations;

public interface IReservationDomain
{
    Task<ResponseViewModel> Create(CreateReservationViewModel reservationVm);
    Task<ResponseViewModel> Update(UpdateReservationViewModel reservationVm);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListResponseViewModel> GetListReservations(GetListReservationParameters parameters);
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
            var reservation = _mapper.Map<Reservation>(createReservationViewModel);

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

    public async Task<BaseListResponseViewModel> GetListReservations(GetListReservationParameters parameters)
    {
        try
        {
            var reservationQuery = _reservationRepository.DbSet().AsNoTracking()
                .ApplySort(parameters.OrderBy)
                .WhereIf(!string.IsNullOrEmpty(parameters.Email),
                    ContainsIgnoreCase<Reservation>(nameof(Reservation.Email), parameters.Email))
                .WhereIf(!string.IsNullOrEmpty(parameters.FullName),
                    ContainsIgnoreCase<Reservation>(new[]{nameof(Reservation.Firstname), nameof(Reservation.Lastname)}, parameters.FullName))
                .WhereIf(!string.IsNullOrEmpty(parameters.FurtherRequest),
                    ContainsIgnoreCase<Reservation>(nameof(Reservation.FurtherRequest), parameters.FurtherRequest))
                .WhereIf(!string.IsNullOrEmpty(parameters.PhoneNumber),
                    r => r.PhoneNumber.Contains(parameters.PhoneNumber))
                .WhereIf(parameters.Statuses is not null && parameters.Statuses.Any(), r => parameters.Statuses.Contains(r.Status))
                .WhereIf(parameters.BookingDateFrom is not null, r => r.BookingDate >= parameters.BookingDateFrom)
                .WhereIf(parameters.BookingDateTo is not null, r => r.BookingDate <= parameters.BookingDateTo)
                .WhereIf(parameters.NoOfPeople is not null, r => r.NoOfPeople == parameters.NoOfPeople);

            return new BaseListResponseViewModel
            {
                Payload = await reservationQuery
                    .Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
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
                Total = await reservationQuery.CountAsync(),
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