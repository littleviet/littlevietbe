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

public class GetListServingParametersValidator : AbstractValidator<GetListServingParameters>
{
    public GetListServingParametersValidator()
    {
        Include(new BaseListQueryParametersValidator());
        RuleFor(x => x.NumberOfPeople).GreaterThan(0).When(x => x.NumberOfPeople is not null);
        RuleFor(x => x.PriceFrom)
            .Must(((model, priceFrom) => model.PriceTo > priceFrom))
            .When(model => model.PriceFrom is not null && model.PriceTo is not null)
            .WithMessage($"Price from must be greater than Price to");
        RuleFor(x => x.PriceFrom).GreaterThan(0).When(x => x.PriceFrom is not null);
        RuleFor(x => x.PriceTo).GreaterThan(0).When(x => x.PriceTo is not null);
    }
}