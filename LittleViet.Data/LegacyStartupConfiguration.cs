using AutoMapper;
using LittleViet.Data.Domains;
using LittleViet.Data.Domains.Account;
using LittleViet.Data.Domains.Coupon;
using LittleViet.Data.Domains.LandingPage;
using LittleViet.Data.Domains.Order;
using LittleViet.Data.Domains.Payment;
using LittleViet.Data.Domains.Products;
using LittleViet.Data.Domains.ProductType;
using LittleViet.Data.Domains.Reservations;
using LittleViet.Data.Domains.Serving;
using LittleViet.Data.Domains.TakeAway;
using LittleViet.Data.Models;
using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Azure.AzureBlobStorage.Interface;
using LittleViet.Infrastructure.Azure.AzureBlobStorage.Service;
using LittleViet.Infrastructure.Email.Interface;
using LittleViet.Infrastructure.Email.Service;
using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Models;
using LittleViet.Infrastructure.Stripe.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Stripe;
using Stripe.Checkout;
using Account = LittleViet.Data.Models.Account;
using Coupon = LittleViet.Data.Models.Coupon;
using Order = LittleViet.Data.Models.Order;
using Product = LittleViet.Data.Models.Product;
using ProductType = LittleViet.Data.Models.ProductType;
using Reservation = LittleViet.Data.Models.Reservation;
using Serving = LittleViet.Data.Models.Serving;
using OrderDetail = LittleViet.Data.Models.OrderDetail;

namespace LittleViet.Data;

public static partial class LegacyStartupConfiguration
{
    private static IMapper Mapper { get; set; }
    private static readonly List<Action<IMapperConfigurationExpression>> MapperConfigs = new();


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
        try
        {
            mapConfig.AssertConfigurationIsValid();
        }
        catch
        {
            //TODO: do something about this
        }
    }

    private static IServiceCollection ConfigureIoC(this IServiceCollection services)
    {
        services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<DbContext, LittleVietContext>();

        services.Scan(scan =>
            scan.FromAssembliesOf(typeof(BaseDomain))
                .AddClasses(x => x.Where(type => 
                    type.Name.EndsWith("Repository") ||
                    type.Name.EndsWith("Domain")
                    ))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.AddScoped(_ => new ProductService())
            .AddScoped(_ => new PriceService())
            .AddScoped(_ => new SessionService())
            .AddScoped<IStripePaymentService, StripePaymentService>()
            .AddScoped<IStripeProductService, StripeProductService>()
            .AddScoped<IStripePriceService, StripePriceService>()
            .AddScoped<IBlobProductImageService, BlobProductImageService>()
            .AddScoped<IEmailService, EmailService>()
            .AddScoped<ITemplateService, TemplateService>();

        return services;
    }

    public static IServiceCollection ConfigureLegacy(this IServiceCollection services)
    {
        MapperConfigs.Add(cfg =>
        {
            cfg.CreateMap<Account, GenericAccountViewModel>().ReverseMap();
            cfg.CreateMap<Account, CreateAccountViewModel>().ReverseMap();
            cfg.CreateMap<Account, UpdateAccountViewModel>().ReverseMap();
            cfg.CreateMap<Account, AccountDetailsViewModel>().ReverseMap();
            cfg.CreateMap<ProductType, CreateProductTypeViewModel>().ReverseMap();
            cfg.CreateMap<ProductType, UpdateProductTypeViewModel>().ReverseMap();
            cfg.CreateMap<ProductType, ProductLandingPageViewModel>().ReverseMap();
            cfg.CreateMap<ProductType, ProductTypeDetailsViewModel>().ReverseMap();
            cfg.CreateMap<Product, CreateProductViewModel>().ReverseMap();
            cfg.CreateMap<Product, UpdateProductViewModel>().ReverseMap();
            cfg.CreateMap<Product, ProductsLandingPageViewModel>().ReverseMap();
            cfg.CreateMap<ProductImage, CreateProductImageViewModel>().ReverseMap();
            cfg.CreateMap<Coupon, CreateCouponViewModel>().ReverseMap();
            cfg.CreateMap<Order, CreateOrderViewModel>().ReverseMap();
            cfg.CreateMap<Order, UpdateOrderViewModel>().ReverseMap();
            cfg.CreateMap<Order, OrderDetailsViewModel>().ReverseMap();
            cfg.CreateMap<OrderDetail, CreateOrderDetailViewModel>().ReverseMap();
            cfg.CreateMap<OrderDetail, OrderDetailItemViewModel>().ReverseMap();
            cfg.CreateMap<Serving, CreateServingViewModel>().ReverseMap();
            cfg.CreateMap<Serving, UpdateServingViewModel>().ReverseMap();
            cfg.CreateMap<Serving, ServingViewDetailsModel>().ReverseMap();
            cfg.CreateMap<Coupon, CouponDetailsViewModel>().ReverseMap();            
            cfg.CreateMap<CouponType, CreateCouponTypeViewModel>().ReverseMap();
            cfg.CreateMap<Reservation, ReservationDetailsViewModel>().ReverseMap();
            cfg.CreateMap<Reservation, UpdateReservationViewModel>().ReverseMap();
            cfg.CreateMap<Reservation, CreateReservationViewModel>().ReverseMap();

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