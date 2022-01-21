﻿using AutoMapper;
using LittleViet.Data.Domains;
using LittleViet.Data.Models;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Models;
using LittleViet.Infrastructure.Stripe.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using Stripe.Checkout;
using Account = LittleViet.Data.Models.Account;
using Coupon = LittleViet.Data.Models.Coupon;
using Order = LittleViet.Data.Models.Order;
using Product = LittleViet.Data.Models.Product;

namespace LittleViet.Data.Global;

public static partial class StartupConfiguration
{
    public static IMapper Mapper { get; private set; }
    private static List<Action<IMapperConfigurationExpression>> MapperConfigs
        = new List<Action<IMapperConfigurationExpression>>();



    private static void ConfigureAutomapper()
    {
        var mapConfig = new MapperConfiguration(cfg =>
        {
            foreach (var c in MapperConfigs)
            {
                c.Invoke(cfg);
            }
        });

        Mapper = mapConfig.CreateMapper();

    }

    private static IServiceCollection ConfigureIoC(this IServiceCollection services)
    {
        services.AddScoped<UnitOfWork>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<DbContext, LittleVietContext>()
            .AddScoped<IAccountRepository, AccountRepository>()
            .AddScoped<ICouponRepository, CouponRepository>()
            .AddScoped<IOrderDetailRepository, OrderDetailRepository>()
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<IProductTypeRepository, ProductTypeRepository>()
            .AddScoped<IProductImageRepository, ProductImageRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IServingRepository, ServingRepository>()
            .AddScoped<IReservationRepository, ReservationRepository>();

        services.AddScoped<IAccountDomain, AccountDomain>()
            .AddScoped<ICouponDomain, CouponDomain>()
            .AddScoped<IOrderDomain, OrderDomain>()
            .AddScoped<IOrderDetailDomain, OrderDetailDomain>()
            .AddScoped<IProductDomain, ProductDomain>()
            .AddScoped<IProductImageDomain, ProductImageDomain>()
            .AddScoped<IProductTypeDomain, ProductTypeDomain>()
            .AddScoped<ILandingPageDomain, LandingPageDomain>()
            .AddScoped<IReservationDomain, ReservationDomain>()
            .AddScoped<IServingDomain, ServingDomain>()
            .AddScoped<IPaymentDomain, PaymentDomain>();
        
        services.AddScoped<ProductService>(s => new ProductService())
            .AddScoped<PriceService>(s => new PriceService())
            .AddScoped<SessionService>(s => new SessionService())
            .AddScoped<IStripePaymentService, StripePaymentService>()
            .AddScoped<IStripeProductService, StripeProductService>()
            .AddScoped<IStripePriceService, StripePriceService>();

        return services;
    }

    public static IServiceCollection ConfigureLegacy(this IServiceCollection services)
    {
        MapperConfigs.Add(cfg =>
        {
            cfg.CreateMap<Account, AccountViewModel>().ReverseMap();
            cfg.CreateMap<Account, CreateAccountViewModel>().ReverseMap();
            cfg.CreateMap<Account, UpdateAccountViewModel>().ReverseMap();
            cfg.CreateMap<ProductType, CreateProductTypeViewModel>().ReverseMap();
            cfg.CreateMap<ProductType, UpdateProductTypeViewModel>().ReverseMap();
            cfg.CreateMap<ProductType, ProductLandingPageViewModel>().ReverseMap();
            cfg.CreateMap<Product, ProductsLandingPageViewModel>().ReverseMap();
            cfg.CreateMap<Product, CreateProductViewModel>().ReverseMap();
            cfg.CreateMap<Product, UpdateProductViewModel>().ReverseMap();
            cfg.CreateMap<Product, ProductsLandingPageViewModel>().ReverseMap();
            cfg.CreateMap<Coupon, CreateCouponViewModel>().ReverseMap();
            cfg.CreateMap<Order, CreateOrderViewModel>().ReverseMap();
            cfg.CreateMap<Order, UpdateOrderViewModel>().ReverseMap();
            cfg.CreateMap<OrderDetail, CreateOrderDetailViewModel>().ReverseMap();
            cfg.CreateMap<Serving, CreateServingViewModel>().ReverseMap();
            cfg.CreateMap<Serving, UpdateServingViewModel>().ReverseMap();
            
            cfg.CreateMap<UpdateProductViewModel, UpdateProductDto>().ReverseMap();
            cfg.CreateMap<CreateProductViewModel, CreateProductDto>().ReverseMap();
            cfg.CreateMap<CreateServingViewModel, CreatePriceDto>().ReverseMap();
        });

        ConfigureAutomapper();
        services.AddSingleton(Mapper);
        services.ConfigureIoC();
        return services;
    }
}

