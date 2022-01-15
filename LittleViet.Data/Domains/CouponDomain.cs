using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Models.Repositories;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Domains;

public interface ICouponDomain
{
    Task<ResponseViewModel> Create(CreateCouponViewModel createCouponViewModel);
    Task<ResponseViewModel> Update(UpdateCouponViewModel updateCouponViewModel);
    Task<ResponseViewModel> UpdateStatus(UpdateCouponStatusViewModel updateCouponStatusViewModel);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListQueryResponseViewModel> GetListCoupons(BaseListQueryParameters parameters);
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

    public async Task<BaseListQueryResponseViewModel> GetListCoupons(BaseListQueryParameters parameters)
    {
        try
        {
            var productTypes = _couponRepository.DbSet().AsNoTracking();

            return new BaseListQueryResponseViewModel
            {
                Payload = await productTypes.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber).ToListAsync(),
                Success = true,
                Total = await productTypes.CountAsync(),
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

            if (coupon == null)
            {
                return new ResponseViewModel { Success = false, Message = "This coupon does not exist" };
            }

            return new ResponseViewModel { Success = true, Payload = coupon };
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

