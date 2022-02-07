using FluentValidation;
using LittleViet.Data.Models;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains.Reservations;

public class CreateReservationViewModelValidator : AbstractValidator<CreateReservationViewModel>
{
    public CreateReservationViewModelValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.BookingDate).NotEmpty().GreaterThan(DateTime.UtcNow);
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.NoOfPeople).GreaterThan(0);
    }
}

public class UpdateReservationViewModelValidator : AbstractValidator<UpdateReservationViewModel>
{
    public UpdateReservationViewModelValidator()
    {
        // RuleFor(x => x.Id).NotEmpty(); TODO: use Body And Route ModelBinder for this, implement later
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