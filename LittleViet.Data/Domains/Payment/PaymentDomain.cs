using AutoMapper;
using LittleViet.Data.Models.Global;
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
    private readonly IMapper _mapper;
    private readonly IStripePaymentService _stripePaymentService;
    private readonly IOrderDomain _orderDomain;

    public PaymentDomain(IUnitOfWork uow, IMapper mapper, IStripePaymentService stripePaymentService, IOrderDomain orderRepository) : base(uow)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _stripePaymentService = stripePaymentService ?? throw new ArgumentNullException(nameof(stripePaymentService));
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
        return await _orderDomain.HandleSuccessfulOrder(orderGuid, session.Id);
    }
}

