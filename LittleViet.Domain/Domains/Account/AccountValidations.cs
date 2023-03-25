using FluentValidation;
using LittleViet.Domain.ViewModels;

namespace LittleViet.Domain.Domains.Account;

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

public class UpdatePasswordViewModelValidator : AbstractValidator<UpdatePasswordViewModel> 
{
    public UpdatePasswordViewModelValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ConfirmPassword).Length(5, 64)
            .Must(((model, confirmPassword) => model.NewPassword == confirmPassword))
            .WithMessage("Password confirmations not matched!");
        RuleFor(x => x.NewPassword).Length(5, 64);
        RuleFor(x => x.OldPassword).Length(5, 64);
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

public class GetListAccountParametersValidator : AbstractValidator<GetListAccountParameters> 
{
    public GetListAccountParametersValidator() 
    {
        Include(new BaseListQueryParametersValidator<Models.Account>());
        RuleFor(x => x.Email).EmailAddress();
        RuleForEach(x => x.AccountTypes).IsInEnum();
    }
}