using AutoMapper;
using LittleViet.Data.Domains;
using LittleViet.Data.Models;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Models.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Global
{
    public static partial class StartupConfiguration
    {
        public static IMapper Mapper { get; private set; }
        private static List<Action<IMapperConfigurationExpression>> MapperConfigs
            = new List<Action<IMapperConfigurationExpression>>();

        private static void ConfigureAutomapper()
        {
            //AutoMapper
            var mapConfig = new MapperConfiguration(cfg =>
            {
                foreach (var c in MapperConfigs)
                {
                    c.Invoke(cfg);
                }
            });
            StartupConfiguration.Mapper = mapConfig.CreateMapper();

        }

        private static void ConfigureIoC(IServiceCollection services)
        {
            //IoC
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

        }

        public static void Configure(IServiceCollection services)
        {
            MapperConfigs.Add(cfg =>
            {
                cfg.CreateMap<Account, AccountVM>().ReverseMap();
                cfg.CreateMap<Coupon, CouponVM>().ReverseMap();
                cfg.CreateMap<Order, OrderVM>().ReverseMap();
                cfg.CreateMap<OrderDetail, OrderDetailVM>().ReverseMap();
                cfg.CreateMap<Product, ProductVM>().ReverseMap();
                cfg.CreateMap<ProductImage, ProductImageVM>().ReverseMap();
                cfg.CreateMap<ProductType, ProductTypeVM>().ReverseMap();
                cfg.CreateMap<Reservation, ReservationVM>().ReverseMap();
                cfg.CreateMap<Serving, ServingVM>().ReverseMap();

            });

            ConfigureAutomapper();
            services.AddSingleton(Mapper);
            services.AddDbContext<LittleVietContext>();
            ConfigureIoC(services);

            //extra
            services.AddScoped<IAccountDomain, AccountDomain>()
            .AddScoped<ICouponDomain, CouponDomain>()
            .AddScoped<IOrderDomain, OrderDomain>()
            .AddScoped<IOrderDetailDomain, OrderDetailDomain>()
            .AddScoped<IProductDomain, ProductDomain>()
            .AddScoped<IProductImageDomain, ProductImageDomain>()
            .AddScoped<IProductTypeDomain, ProductTypeDomain>()
            .AddScoped<IReservationDomain, ReservationDomain>()
            .AddScoped<IServingDomain, ServingDomain>();

        }
    }
}
