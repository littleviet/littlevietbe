using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.EntityFramework;
using LittleViet.Infrastructure.Stripe;
using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;

namespace LittleViet.Data.Domains.Coupon;

public interface ICouponTypeDomain
{
    Task<ResponseViewModel> CreateCouponType(CreateCouponTypeViewModel createCouponTypeViewModel);
    Task<ResponseViewModel> GetCouponTypes();
}
internal class CouponTypeDomain : BaseDomain, ICouponTypeDomain
{
    private readonly ICouponTypeRepository _couponTypeRepository;
    private readonly IStripePriceService _stripePriceService;
    private readonly IMapper _mapper;
    private readonly StripeSettings _stripeSettings;
    public CouponTypeDomain(IOptions<StripeSettings> stripeSettings, IUnitOfWork uow, IMapper mapper, IStripePriceService stripePriceService, ICouponTypeRepository couponTypeRepository) : base(uow)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _stripePriceService = stripePriceService ?? throw new ArgumentNullException(nameof(stripePriceService));
        _couponTypeRepository = couponTypeRepository ?? throw new ArgumentNullException(nameof(couponTypeRepository));
        _stripeSettings = stripeSettings.Value ?? throw new ArgumentNullException(nameof(stripeSettings));
    }

    public async Task<ResponseViewModel> CreateCouponType(CreateCouponTypeViewModel createCouponTypeViewModel)
    {
        var couponType = _mapper.Map<CouponType>(createCouponTypeViewModel);
        couponType.Id = Guid.NewGuid();

        await using var transaction = _uow.BeginTransation();
        try
        {
            _ = _couponTypeRepository.Add(couponType);
            await _uow.SaveAsync();

            var createStripePriceDto = new CreatePriceDto()
            {
                Price = (long) couponType.Value * 100,
                StripeProductId = _stripeSettings.CouponProductId,
                Metadata = new() {{Infrastructure.Stripe.Payment.CouponTypePriceMetaDataKey, couponType.Id.ToString()}},
            };

            var stripePrice = await _stripePriceService.CreatePrice(createStripePriceDto);
            couponType.StripePriceId = stripePrice.Id;

            await _uow.SaveAsync();
            await transaction.CommitAsync();

            return new ResponseViewModel {Success = true, Message = "Create successful"};
        }
        catch (StripeException se)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ResponseViewModel> GetCouponTypes()
    {

        try
        {
            var couponTypes = await _couponTypeRepository.DbSet().Select(x => new GetCouponTypeViewModel()
            {
                Id = x.Id,
                Value = x.Value,
                Name = x.Name,
                Currency = _stripeSettings.Payment.Currency,
            }).ToListAsync();
            return new ResponseViewModel {Success = true, Message = "Create successful", Payload = couponTypes};
        }
        catch (Exception e)
        {
            throw;
        }
    }
}

