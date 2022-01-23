using AutoMapper;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Repositories;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using Coupon = LittleViet.Data.Models.Coupon;

namespace LittleViet.Data.Domains;

public interface ICouponDomain
{
    Task<ResponseViewModel> Create(CreateCouponViewModel createCouponViewModel);
    Task<ResponseViewModel> Update(UpdateCouponViewModel updateCouponViewModel);
    Task<ResponseViewModel> UpdateStatus(UpdateCouponStatusViewModel updateCouponStatusViewModel);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListResponseViewModel> GetListCoupons(BaseListQueryParameters parameters);
    Task<BaseListResponseViewModel> Search(BaseSearchParameters parameters);
    Task<ResponseViewModel> GetCouponById(Guid id);
}
internal class CouponDomain : BaseDomain, ICouponDomain
{
    private readonly ICouponRepository _couponRepository;
    private readonly IMapper _mapper;
    public CouponDomain(IUnitOfWork uow, ICouponRepository couponRepository, IMapper mapper) : base(uow)
    {
        _couponRepository = couponRepository ?? throw new ArgumentNullException(nameof(couponRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ResponseViewModel> Create(CreateCouponViewModel createCouponViewModel)
    {
        try
        {
            var coupon = _mapper.Map<Coupon>(createCouponViewModel);
            var date = DateTime.UtcNow;

            coupon.Id = Guid.NewGuid();
            coupon.UpdatedBy = createCouponViewModel.CreatedBy;
            coupon.UpdatedDate = date;
            coupon.CreatedDate = date;
            coupon.IsDeleted = false;
            coupon.CouponCode = GenerateCouponCode();
            coupon.Status = CouponStatus.Created;

            _couponRepository.Add(coupon);
            await _uow.SaveAsync();

            return new ResponseViewModel { Success = true, Message = "Create successful", Payload = coupon.CouponCode };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> Update(UpdateCouponViewModel updateCouponViewModel)
    {
        try
        {
            var existedCoupon = await _couponRepository.GetById(updateCouponViewModel.Id);
            if (existedCoupon != null)
            {
                existedCoupon.Status = updateCouponViewModel.Status;
                existedCoupon.PhoneNumber = updateCouponViewModel.PhoneNumber;
                existedCoupon.Email = updateCouponViewModel.Email;
                existedCoupon.Amount = updateCouponViewModel.Amount;
                existedCoupon.UpdatedDate = DateTime.UtcNow;

                _couponRepository.Modify(existedCoupon);
                await _uow.SaveAsync();

                return new ResponseViewModel { Success = true, Message = "Update successful" };
            }

            return new ResponseViewModel { Success = false, Message = "This coupon does not exist" };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> Deactivate(Guid id)
    {
        try
        {
            var coupon = await _couponRepository.GetById(id);
            if (coupon != null)
            {
                _couponRepository.Deactivate(coupon);
                await _uow.SaveAsync();

                return new ResponseViewModel { Success = true, Message = "Delete successful" };
            }

            return new ResponseViewModel { Success = false, Message = "This coupon does not exist" };
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

                return new ResponseViewModel { Success = true, Message = "Update successful" };
            }

            return new ResponseViewModel { Success = false, Message = "This coupon does not exist" };
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
                    Amount = q.Amount,
                    CouponCode = q.CouponCode,
                    Email = q.Email,
                    PhoneNumber = q.PhoneNumber,
                    Status = q.Status,
                    StatusName = q.Status.ToString()
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
                .Where(p => p.CouponCode.ToLower().Contains(keyword) || p.Email.ToLower().Contains(keyword) || p.PhoneNumber.ToLower().Contains(keyword));

            return new BaseListResponseViewModel
            {
                Payload = await coupons
                    .Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                    .Select(q => new CouponViewModel()
                    {
                        Id = q.Id,
                        Amount = q.Amount,
                        CouponCode = q.CouponCode,
                        Email = q.Email,
                        PhoneNumber = q.PhoneNumber,
                        Status = q.Status,
                        StatusName = q.Status.ToString()
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

            if (coupon == null)
            {
                return new ResponseViewModel { Success = false, Message = "This coupon does not exist" };
            }

            return new ResponseViewModel { Success = true, Payload = couponDetails };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    private string GenerateCouponCode()
    {
        //TODO: fix this logic for collision
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        var result = new string(
            Enumerable.Repeat(chars, 10)
                      .Select(s => s[random.Next(s.Length)])
                      .ToArray());

        return result;
    }
}

