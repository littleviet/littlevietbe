using FluentValidation;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains.Account;

public class CreateAccountViewModelValidator : AbstractValidator<CreateAccountViewModel> 
{
    public CreateAccountViewModelValidator() 
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Firstname).NotNull();
        RuleFor(x => x.Lastname).NotNull();
        RuleFor(x => x.Password).NotNull().Length(5, 64);
        RuleFor(x => x.PhoneNumber1).NotNull();
        RuleFor(x => x.AccountType).IsInEnum();
    }
}

public class LoginViewModelValidator : AbstractValidator<LoginViewModel> 
{
    public LoginViewModelValidator() 
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Password).NotNull().Length(5, 64);
    }
}