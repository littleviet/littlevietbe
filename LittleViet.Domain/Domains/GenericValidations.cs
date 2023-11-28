using FluentValidation;
using LittleViet.Domain.Repositories;
using LittleViet.Domain.ViewModels;
using LittleViet.Infrastructure.DateTime;

namespace LittleViet.Domain.Domains;

public static class CustomRules
{
    public static IRuleBuilderOptions<T, DateTime> WithinOpeningTime<T>(this IRuleBuilder<T, DateTime> ruleBuilder,
        IDateTimeService dateTimeService, IVacationRepository vacationRepository)
    {
        return ruleBuilder
            .Must(x => Constants.WorkingWeekDays.Contains(dateTimeService.ConvertToTimeZone(x).DayOfWeek))
            .WithMessage("We are closed on Tue")
            .Must(x =>
            {
                var timePart = TimeOnly.FromDateTime(dateTimeService.ConvertToTimeZone(x));
                return Constants.OpeningHours.Any(x => x.Start <= timePart && timePart <= x.End);
            })
            .WithMessage("Your requested time is not within our opening hours")
            .GreaterThan(DateTime.UtcNow)
            .MustAsync(async (x, ct) =>
            {
                var bookingTime = dateTimeService.ConvertToTimeZone(x);
                var vacation = await vacationRepository.GetByTimeAsync(bookingTime, ct);
                return vacation == null;
            })
            .WithMessage((_, date) =>
                $"We are on vacation during your requested time of {dateTimeService.ConvertToTimeZone(date).ToString("f")}");
    }
}

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
        var properties = orderBy.Split(',').Select(x =>
            x.Replace(" desc", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace(" asc", "", StringComparison.InvariantCultureIgnoreCase));
        return (properties.All(x => !string.IsNullOrEmpty(x)), properties.ToArray());
    }
}