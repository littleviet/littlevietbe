using AutoMapper;
using LittleViet.Data.Domains;
using LittleViet.Data.Models;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Models.Repositories;
using LittleViet.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LittleViet.Data.Global;

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
                cfg.CreateMap<Account, CreateAccountVM>().ReverseMap();
                cfg.CreateMap<Account, UpdateAccountVM>().ReverseMap();
                cfg.CreateMap<ProductType, CreateProductTypeVM>().ReverseMap();
                cfg.CreateMap<ProductType, UpdateProductTypeVM>().ReverseMap();
                cfg.CreateMap<Product, ProductsLP>().ReverseMap();
                cfg.CreateMap<Product, CreateProductVM>().ReverseMap();
                cfg.CreateMap<Product, UpdateProductVM>().ReverseMap();

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

