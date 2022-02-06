using FluentValidation;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains.Products;

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
        RuleForEach(x => x.ProductImages).NotEmpty();
    }
}

public class UpdateProductViewModelValidator : AbstractValidator<UpdateProductViewModel> 
{
    public UpdateProductViewModelValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Name).Length(2, 100);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.CaName).Length(2, 100);
        RuleFor(x => x.EsName).Length(2, 100);
        RuleFor(x => x.MainImage).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ProductTypeId).NotEmpty();
        RuleForEach(x => x.ProductImages).NotEmpty();
    }
}

public class GetListProductParametersValidator : AbstractValidator<GetListProductParameters> 
{
    public GetListProductParametersValidator()
    {
        Include(new BaseListQueryParametersValidator());
        RuleForEach(x => x.Statuses).IsInEnum();
    }
}