﻿using LittleViet.Infrastructure.Stripe.Models;
using Stripe.Checkout;

namespace LittleViet.Infrastructure.Stripe.Interface;

public interface IStripePaymentService
{
    Task<Session> CreateOrderCheckoutSession(CreateSessionDto dto);
    Task<Session> CreateCouponCheckoutSession(CreateSessionDto dto);

}