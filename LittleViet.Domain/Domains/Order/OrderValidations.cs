using FluentValidation;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.DateTime;

namespace LittleViet.Data.Domains.Order;

public class CreateOrderViewModelValidator : AbstractValidator<CreateOrderViewModel>
{
    public CreateOrderViewModelValidator(IDateTimeService dateTimeService)
    {
        RuleFor(x => x.PickupTime)
            .Cascade(CascadeMode.Stop)
            .Must(x => Constants.WorkingWeekDays.Contains(dateTimeService.ConvertToTimeZone(x).DayOfWeek))
            .WithMessage("We are closed on Tue")
            .GreaterThan(DateTime.UtcNow);
        RuleFor(x => x.OrderType).IsInEnum();
        RuleFor(x => x.TotalPrice).NotNull();
        RuleFor(x => x.PaymentType).IsInEnum();
        RuleForEach(x => x.OrderDetails)
            .ChildRules(detail =>
            {
                detail.RuleFor(x => x.Quantity).GreaterThan(0);
                detail.RuleFor(x => x.ServingId).NotEmpty();
            });
    }
}

public class GetListOrderParametersValidator : AbstractValidator<GetListOrderParameters>
{
    public GetListOrderParametersValidator()
    {
        Include(new BaseListQueryParametersValidator<Models.Order>());
        RuleForEach(x => x.PaymentTypes).IsInEnum();
        RuleForEach(x => x.OrderTypes).IsInEnum();
        RuleFor(x => x.PickupTimeFrom)
            .Must(((model, pickupTimeFrom) => model.PickupTimeTo > pickupTimeFrom))
            .When(model => model.PickupTimeFrom is not null && model.PickupTimeTo is not null)
            .WithMessage("Pickup time from must be greater than Pickup time to");
        RuleFor(x => x.TotalPriceFrom)
            .Must(((model, totalPriceFrom) => model.TotalPriceTo > totalPriceFrom))
            .When(model => model.TotalPriceFrom is not null && model.TotalPriceTo is not null)
            .WithMessage("Total price from must be greater than Total price to");
        RuleFor(x => x.TotalPriceFrom).GreaterThan(0).When(x => x.TotalPriceFrom is not null);
        RuleFor(x => x.TotalPriceTo).GreaterThan(0).When(x => x.TotalPriceTo is not null);
    }
}