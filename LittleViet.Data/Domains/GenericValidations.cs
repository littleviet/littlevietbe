using FluentValidation;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains;

public class BaseListQueryParametersValidator : AbstractValidator<BaseListQueryParameters> 
{
    public BaseListQueryParametersValidator() 
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}

public class BaseSearchParametersParametersValidator : AbstractValidator<BaseSearchParameters> 
{
    public BaseSearchParametersParametersValidator() 
    {
        RuleFor(x => x.Keyword).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}