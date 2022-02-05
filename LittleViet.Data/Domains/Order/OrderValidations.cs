using FluentValidation;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains.Order;

public class CreateOrderViewModelValidator : AbstractValidator<CreateOrderViewModel>
{
    public CreateOrderViewModelValidator()
    {
        RuleFor(x => x.PickupTime).GreaterThan(DateTime.UtcNow);
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
        Include(new BaseListQueryParametersValidator());
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
    }
}