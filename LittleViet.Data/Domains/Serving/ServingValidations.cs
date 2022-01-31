using FluentValidation;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains.Serving;

public class CreateServingViewModelValidator : AbstractValidator<CreateServingViewModel> 
{
    public CreateServingViewModelValidator() 
    {
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Name).Length(2, 20);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.NumberOfPeople).GreaterThan(0);
    }
}

public class UpdateServingViewModelValidator : AbstractValidator<UpdateServingViewModel> 
{
    public UpdateServingViewModelValidator() 
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Name).Length(2, 20);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.NumberOfPeople).GreaterThan(0);
    }
}