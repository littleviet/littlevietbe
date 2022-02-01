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