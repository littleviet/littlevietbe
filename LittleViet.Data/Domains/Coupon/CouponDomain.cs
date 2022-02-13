﻿using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Email.Interface;
using LittleViet.Infrastructure.Email.Models;
using LittleViet.Infrastructure.EntityFramework;
using LittleViet.Infrastructure.Stripe;
using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;

namespace LittleViet.Data.Domains.Coupon;

public interface ICouponDomain
{
    Task<ResponseViewModel> CreateCoupon(CreateCouponViewModel createCouponViewModel);
    Task<ResponseViewModel> UpdateCoupon(UpdateCouponViewModel updateCouponViewModel);
    Task<ResponseViewModel> UpdateStatus(UpdateCouponStatusViewModel updateCouponStatusViewModel);
    Task<ResponseViewModel> DeactivateCoupon(Guid id);
    Task<BaseListResponseViewModel> GetListCoupons(BaseListQueryParameters parameters);
    Task<BaseListResponseViewModel> Search(BaseSearchParameters parameters);
    Task<ResponseViewModel> GetCouponById(Guid id);
    Task<ResponseViewModel> HandleSuccessfulCouponPurchase(Guid couponId, string stripeSessionId);
}

internal class CouponDomain : BaseDomain, ICouponDomain
{
    private readonly ICouponRepository _couponRepository;
    private readonly IStripePaymentService _stripePaymentService;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly ITemplateService _templateService;

    public CouponDomain(IOptions<StripeSettings> stripeSettings, IUnitOfWork uow, ICouponRepository couponRepository,
        IMapper mapper, IStripePaymentService stripePaymentService, IEmailService emailService,
        ITemplateService templateService) : base(uow)
    {
        _couponRepository = couponRepository ?? throw new ArgumentNullException(nameof(couponRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _stripePaymentService = stripePaymentService ?? throw new ArgumentNullException(nameof(stripePaymentService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
    }

    public async Task<ResponseViewModel> CreateCoupon(CreateCouponViewModel createCouponViewModel)
    {
        var coupon = _mapper.Map<Models.Coupon>(createCouponViewModel);
        var couponGuid = Guid.NewGuid();

        coupon.Id = couponGuid;
        coupon.Status = CouponStatus.Created;
        coupon.CurrentQuantity = createCouponViewModel.Quantity;
        coupon.InitialQuantity = createCouponViewModel.Quantity;

        await using var transaction = _uow.BeginTransation();

        try
        {
            var entry = _couponRepository.Add(coupon);
            await _uow.SaveAsync();

            await entry.Reference(e => e.CouponType).LoadAsync();

            var stripeSessionDto = new CreateSessionDto()
            {
                Metadata = new() {{Infrastructure.Stripe.Payment.CouponCheckoutMetaDataKey, couponGuid.ToString()}},
                SessionItems = new List<SessionItem>()
                {
                    new()
                    {
                        Quantity = entry.Entity.InitialQuantity,
                        StripePriceId = entry.Entity.CouponType.StripePriceId,
                    }
                }
            };

            var checkoutSessionResult = await _stripePaymentService.CreateCouponCheckoutSession(stripeSessionDto);
            coupon.LastStripeSessionId = checkoutSessionResult.Id;

            await _uow.SaveAsync();
            await transaction.CommitAsync();

            return new ResponseViewModel
            {
                Success = true,
                Message = "Create successful",
                Payload = new
                {
                    OrderId = couponGuid.ToString(),
                    Url = checkoutSessionResult.Url,
                    SessionId = checkoutSessionResult.Id,
                },
            };
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

    public async Task<ResponseViewModel> HandleSuccessfulCouponPurchase(Guid couponId, string stripeSessionId)
    {
        await using var transaction = _uow.BeginTransation();

        try
        {
            var coupon = await _couponRepository.GetById(couponId);

            if (coupon is null)
            {
                throw new Exception($"Cannot find coupon of Id {couponId}");
            }

            coupon.Status = CouponStatus.Paid;
            coupon.LastStripeSessionId = stripeSessionId;
            coupon.CouponCode = GenerateCouponCode(coupon.FirstName);

            await _uow.SaveAsync();

            var template = await _templateService.GetTemplateEmail(EmailTemplates.CouponPurchaseSuccess);

            var body = template
                .Replace("{name}", coupon.FirstName)
                .Replace("{time}", coupon.CreatedDate.ToString("hh:mm:ss MM/dd/yyyy"))
                .Replace("{usage-left}", coupon.CurrentQuantity.ToString())
                .Replace("{phone-number}", coupon.PhoneNumber)
                .Replace("{coupon-id}", coupon.Id.ToString())
                .Replace("{email}", coupon.Email)
                .Replace("{coupon-code}", coupon.CouponCode);

            await _emailService.SendEmailAsync(
                body: body,
                toName: coupon.FirstName,
                toAddress: coupon.Email,
                subject: EmailTemplates.CouponPurchaseSuccess.SubjectName
            );

            await _uow.SaveAsync();
            await transaction.CommitAsync();

            return new ResponseViewModel {Success = true};
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ResponseViewModel> UpdateCoupon(UpdateCouponViewModel updateCouponViewModel)
    {
        try
        {
            var existedCoupon = await _couponRepository.GetById(updateCouponViewModel.Id);
            if (existedCoupon != null)
            {
                existedCoupon.Status = updateCouponViewModel.Status;
                existedCoupon.PhoneNumber = updateCouponViewModel.PhoneNumber;
                existedCoupon.Email = updateCouponViewModel.Email;
                // existedCoupon.Amount = updateCouponViewModel.Amount;

                _couponRepository.Modify(existedCoupon);
                await _uow.SaveAsync();

                return new ResponseViewModel {Success = true, Message = "Update successful"};
            }

            return new ResponseViewModel {Success = false, Message = "This coupon does not exist"};
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> DeactivateCoupon(Guid id) // TODO: should not exist
    {
        try
        {
            var coupon = await _couponRepository.GetById(id);
            if (coupon != null)
            {
                _couponRepository.Deactivate(coupon);
                await _uow.SaveAsync();

                return new ResponseViewModel {Success = true, Message = "Delete successful"};
            }

            return new ResponseViewModel {Success = false, Message = "This coupon does not exist"};
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> UpdateStatus(UpdateCouponStatusViewModel updateCouponStatusViewModel)
    {
        try
        {
            var existedCoupon = await _couponRepository.GetById(updateCouponStatusViewModel.Id);
            if (existedCoupon != null)
            {
                existedCoupon.Status = updateCouponStatusViewModel.Status;

                _couponRepository.Modify(existedCoupon);
                await _uow.SaveAsync();

                return new ResponseViewModel {Success = true, Message = "Update successful"};
            }

            return new ResponseViewModel {Success = false, Message = "This coupon does not exist"};
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<BaseListResponseViewModel> GetListCoupons(BaseListQueryParameters parameters)
    {
        try
        {
            var coupons = _couponRepository.DbSet().AsNoTracking();

            return new BaseListResponseViewModel
            {
                Payload = await coupons.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                    .Select(q => new CouponViewModel()
                    {
                        Id = q.Id,
                        // Amount = q.Amount,
                        CouponCode = q.CouponCode,
                        Email = q.Email,
                        PhoneNumber = q.PhoneNumber,
                        Status = q.Status,
                    })
                    .ToListAsync(),
                Success = true,
                Total = await coupons.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<BaseListResponseViewModel> Search(BaseSearchParameters parameters)
    {
        try
        {
            var keyword = parameters.Keyword.ToLower();
            var coupons = _couponRepository.DbSet().AsNoTracking()
                .Where(p => p.CouponCode.ToLower().Contains(keyword) || p.Email.ToLower().Contains(keyword) ||
                            p.PhoneNumber.ToLower().Contains(keyword));

            return new BaseListResponseViewModel
            {
                Payload = await coupons
                    .Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                    .Select(q => new CouponViewModel()
                    {
                        Id = q.Id,
                        // Amount = q.Amount,
                        CouponCode = q.CouponCode,
                        Email = q.Email,
                        PhoneNumber = q.PhoneNumber,
                        Status = q.Status,
                    })
                    .ToListAsync(),
                Success = true,
                Total = await coupons.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> GetCouponById(Guid id)
    {
        try
        {
            var coupon = await _couponRepository.GetById(id);
            var couponDetails = _mapper.Map<CouponDetailsViewModel>(coupon);

            couponDetails.StatusName = coupon.Status.ToString();

            return new ResponseViewModel {Success = true, Payload = couponDetails};
        }
        catch (Exception e)
        {
            throw;
        }
    }

    private string GenerateCouponCode(string firstName)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();

        var head = firstName.Substring(0, 3).ToUpper();

        var subHead = new string(
            Enumerable.Repeat(chars, 4)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());

        var dateTimeBody = DateTime.UtcNow.ToString("yyMMdd-HHmmss");

        return string.Join("-", head, subHead, dateTimeBody);
    }
}