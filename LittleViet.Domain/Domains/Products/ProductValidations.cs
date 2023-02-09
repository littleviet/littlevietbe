using FluentValidation;
using LittleViet.Domain.Models;
using LittleViet.Domain.ViewModels;

namespace LittleViet.Domain.Domains.Products;

public class CreateProductViewModelValidator : AbstractValidator<CreateProductViewModel>
{
    public CreateProductViewModelValidator()
    {
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Name).Length(2, 100);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.CaName).Length(2, 100);
        RuleFor(x => x.EsName).Length(2, 100);
        RuleFor(x => x.MainImage)
            .LessThan(x => x.ProductImages.Count)
            .GreaterThanOrEqualTo(0);
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
        RuleFor(x => x.ProductTypeId).NotEmpty();
    }
}

public class GetListProductParametersValidator : AbstractValidator<GetListProductParameters>
{
    public GetListProductParametersValidator()
    {
        Include(new BaseListQueryParametersValidator<Product>());
        RuleForEach(x => x.Statuses).IsInEnum();
    }
}