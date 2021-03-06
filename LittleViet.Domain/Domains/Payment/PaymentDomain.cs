using LittleViet.Data.Domains.Coupon;
using LittleViet.Data.Domains.Order;
using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using Stripe.Checkout;

namespace LittleViet.Data.Domains.Payment;

public interface IPaymentDomain
{
    Task<ResponseViewModel> HandleSuccessfulPayment(Session session);
    Task<ResponseViewModel> HandleExpiredPayment(Session session);
}

public class PaymentDomain : BaseDomain, IPaymentDomain
{
    private readonly IOrderDomain _orderDomain;
    private readonly ICouponDomain _couponDomain;

    public PaymentDomain(IUnitOfWork uow, IOrderDomain orderDomain, ICouponDomain couponDomain) : base(uow)
    {
        _orderDomain = orderDomain ?? throw new ArgumentNullException(nameof(orderDomain));
        _couponDomain = couponDomain ?? throw new ArgumentNullException(nameof(couponDomain));
    }

    public async Task<ResponseViewModel> HandleSuccessfulPayment(Session session)
    {
        var metadataKey = session.Metadata.Keys.Single(key => key.StartsWith("checkout_"));
        switch (metadataKey)
        {
            case Infrastructure.Stripe.Payment.OrderCheckoutMetaDataKey:
                var orderGuid = Guid.Parse(session.Metadata.GetValueOrDefault(metadataKey));
                return await _orderDomain.HandleSuccessfulOrder(orderGuid, session.Id);
            case Infrastructure.Stripe.Payment.CouponCheckoutMetaDataKey:
                var couponGuid = Guid.Parse(session.Metadata.GetValueOrDefault(metadataKey));
                return await _couponDomain.HandleSuccessfulCouponPurchase(couponGuid, session.Id);
            default:
                throw new InvalidOperationException($"Invalid Payment Operation with metadata key of: {metadataKey}-{session.Metadata.GetValueOrDefault(metadataKey)}");
        }
    }

    public async Task<ResponseViewModel> HandleExpiredPayment(Session session)
    {
        var orderGuid = Guid.Parse(session.Metadata.GetValueOrDefault(Infrastructure.Stripe.Payment.OrderCheckoutMetaDataKey));
        return await _orderDomain.HandleExpiredOrder(orderGuid, session.Id);
    }
}

