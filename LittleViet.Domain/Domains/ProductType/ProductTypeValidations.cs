using FluentValidation;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains.Order;

public class CreateProductTypeModelValidator : AbstractValidator<CreateProductTypeViewModel> 
{
    public CreateProductTypeModelValidator() 
    {
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Name).Length(2, 100);
        RuleFor(x => x.CaName).Length(2, 100);
        RuleFor(x => x.EsName).Length(2, 100);
    }
}

public class UpdateProductTypeModelValidator : AbstractValidator<UpdateProductTypeViewModel> 
{
    public UpdateProductTypeModelValidator() 
    {
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).Length(2, 100);
        RuleFor(x => x.CaName).Length(2, 100);
        RuleFor(x => x.EsName).Length(2, 100);
    }
}

public class GetListProductTypeParametersValidator : AbstractValidator<GetListProductTypeParameters> 
{
    public GetListProductTypeParametersValidator()
    {
        Include(new BaseListQueryParametersValidator<Models.ProductType>());
    }
}