using AutoMapper;
using LittleViet.Data.Domains.Order;
using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Stripe.Interface;
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

    public PaymentDomain(IUnitOfWork uow, IOrderDomain orderRepository) : base(uow)
    {
        _orderDomain = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task<ResponseViewModel> HandleSuccessfulPayment(Session session)
    {
        var orderGuid = Guid.Parse(session.Metadata.GetValueOrDefault(Infrastructure.Stripe.Payment.OrderMetaDataKey));
        return await _orderDomain.HandleSuccessfulOrder(orderGuid, session.Id);
    }


    public async Task<ResponseViewModel> HandleExpiredPayment(Session session)
    {
        var orderGuid = Guid.Parse(session.Metadata.GetValueOrDefault(Infrastructure.Stripe.Payment.OrderMetaDataKey));
        return await _orderDomain.HandleExpiredOrder(orderGuid, session.Id);
    }
}

