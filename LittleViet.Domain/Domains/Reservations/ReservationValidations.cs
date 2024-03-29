﻿using FluentValidation;
using LittleViet.Domain.Models;
using LittleViet.Domain.Repositories;
using LittleViet.Domain.ViewModels;
using LittleViet.Infrastructure.DateTime;

namespace LittleViet.Domain.Domains.Reservations;

public class CreateReservationViewModelValidator : AbstractValidator<CreateReservationViewModel>
{
    public CreateReservationViewModelValidator(IDateTimeService dateTimeService, IVacationRepository vacationRepository)
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.BookingDate)
            .Cascade(CascadeMode.Stop)
            .WithinOpeningTime(dateTimeService, vacationRepository);
        
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.NoOfPeople).GreaterThan(0);
    }
}

public class UpdateReservationViewModelValidator : AbstractValidator<UpdateReservationViewModel>
{
    public UpdateReservationViewModelValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.BookingDate).NotEmpty().GreaterThan(DateTime.UtcNow)
            .When(x => x.Status == ReservationStatus.Reserved);
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.NoOfPeople).GreaterThan(0);
    }
}

public class GetListReservationParametersValidator : AbstractValidator<GetListReservationParameters>
{
    public GetListReservationParametersValidator()
    {
        Include(new BaseListQueryParametersValidator<Reservation>());
        RuleForEach(x => x.Statuses).IsInEnum();
        RuleFor(x => x.NoOfPeople).GreaterThan(0).When(x => x.NoOfPeople > 0);
        RuleFor(x => x.BookingDateFrom)
            .Must(((model, bookingDateFrom) => model.BookingDateTo > bookingDateFrom))
            .When(model => model.BookingDateFrom is not null && model.BookingDateTo is not null)
            .WithMessage("Booking Date from must be greater than Booking Date to");
    }
}