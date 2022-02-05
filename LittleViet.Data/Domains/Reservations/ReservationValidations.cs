using FluentValidation;
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
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.BookingDate).NotEmpty().GreaterThan(DateTime.UtcNow);
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.NoOfPeople).GreaterThan(0);
    }
}