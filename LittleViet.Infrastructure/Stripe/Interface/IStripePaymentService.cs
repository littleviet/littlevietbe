using LittleViet.Infrastructure.Stripe.Models;
using Stripe.Checkout;

namespace LittleViet.Infrastructure.Stripe.Interface;

public interface IStripePaymentService
{
    Task<Session> CreateCheckoutSession(CreateSessionDto dto);
}