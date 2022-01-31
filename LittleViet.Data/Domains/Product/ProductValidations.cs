using FluentValidation;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains.Order;

public class CreateProductViewModelValidator : AbstractValidator<CreateProductViewModel> 
{
    public CreateProductViewModelValidator() 
    {
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Name).Length(2, 100);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.CaName).Length(2, 100);
        RuleFor(x => x.EsName).Length(2, 100);
        RuleFor(x => x.MainImage).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ProductTypeId).NotEmpty();
    }
}