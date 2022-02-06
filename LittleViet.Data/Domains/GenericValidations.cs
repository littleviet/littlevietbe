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

public class BaseListQueryParametersValidator<T> : AbstractValidator<BaseListQueryParameters<T>> where T : class
{
    public BaseListQueryParametersValidator()
    {
        Include(new BaseListQueryParametersValidator());
        RuleFor(x => x.OrderBy)
            .Must((p, orderBy) =>
            {
                var (correctFormat, properties) = IsOrderByCorrectFormat(orderBy);
                return correctFormat 
                       && !properties.Except(p.Properties, StringComparer.InvariantCultureIgnoreCase).Any();
            })
            .WithMessage(x => $"Order By syntax incorrect." +
                         $" Available properties: [{string.Join(", ", x.Properties)}]");
    }

    private (bool, string[]) IsOrderByCorrectFormat(string orderBy)
    {
        var properties = orderBy.Split(',').Select(x => x.Replace(" desc", ""));
        return (properties.All(x => !string.IsNullOrEmpty(x)), properties.ToArray());
    }
}