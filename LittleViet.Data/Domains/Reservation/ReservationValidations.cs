using FluentValidation;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains.Reservation;

public class CreateReservationViewModelValidator : AbstractValidator<CreateReservationViewModel> 
{
    public CreateReservationViewModelValidator() 
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.BookingDate).NotEmpty().GreaterThan(DateTime.Now);
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.NoOfPeople).GreaterThan(0);
    }
}