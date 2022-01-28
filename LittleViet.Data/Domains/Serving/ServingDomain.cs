using AutoMapper;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Models;
using LittleViet.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Domains.Serving;

public interface IServingDomain
{
    Task<ResponseViewModel> Create(CreateServingViewModel createServingViewModel);
    Task<ResponseViewModel> Update(UpdateServingViewModel updateServingViewModel);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListResponseViewModel> GetListServing(BaseListQueryParameters parameters);
    Task<BaseListResponseViewModel> Search(BaseSearchParameters parameters);
    Task<ResponseViewModel> GetServingById(Guid id);
}

internal class ServingDomain : BaseDomain, IServingDomain
{
    private readonly IServingRepository _servingRepository;
    private readonly IStripePriceService _stripePriceService;
    private readonly IMapper _mapper;

    public ServingDomain(IUnitOfWork uow, IServingRepository servingRepository, IMapper mapper,
        IStripePriceService stripePriceService) : base(uow)
    {
        _servingRepository = servingRepository ?? throw new ArgumentNullException(nameof(servingRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _stripePriceService = stripePriceService ?? throw new ArgumentNullException(nameof(stripePriceService));
    }

    public async Task<ResponseViewModel> Create(CreateServingViewModel createServingViewModel)
    {
        try
        {
            var serving = _mapper.Map<Models.Serving>(createServingViewModel);

            var date = DateTime.UtcNow;

            serving.Id = Guid.NewGuid();

            var entry = _servingRepository.Add(serving);
            await _uow.SaveAsync();

            await entry.Reference(e => e.Product).LoadAsync();

            var createStripePriceDto = new CreatePriceDto()
            {
                Price = (long)serving.Price * 100,
                Currency = "eur",
                StripeProductId = entry.Entity.Product.StripeProductId,
            };
            
            var stripePrice = await _stripePriceService.CreatePrice(createStripePriceDto);
            serving.StripePriceId = stripePrice.Id;

            await _uow.SaveAsync();

            return new ResponseViewModel { Success = true, Message = "Create successful" };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<ResponseViewModel> Deactivate(Guid id)
    {
        try
        {
            var serving = await _servingRepository.GetById(id);

            if (serving != null)
            {
                _servingRepository.Deactivate(serving);
                await _uow.SaveAsync();
                await _stripePriceService.DeactivatePrice(serving.StripePriceId);

                return new ResponseViewModel { Success = true, Message = "Delete successful" };
            }

            return new ResponseViewModel { Success = false, Message = "This serving does not exist" };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<ResponseViewModel> Update(UpdateServingViewModel updateServingViewModel)
    {
        try
        {
            var existedServing = await _servingRepository.GetById(updateServingViewModel.Id);

            var pricesDifferent = IsPriceDifferent(existedServing, updateServingViewModel);

            if (pricesDifferent == true)
            {
                var newPrice = await _stripePriceService.UpdatePrice(
                    new UpdatePriceDto
                    {
                        Amount = (long)existedServing.Price * 100,
                        Currency = "eur",
                        Id = existedServing.StripePriceId,
                        ProductId = existedServing.Product.StripeProductId,
                    });
                existedServing.StripePriceId = newPrice.Id;
            }

            if (existedServing != null)
            {
                existedServing.Price = updateServingViewModel.Price;
                existedServing.Description = updateServingViewModel.Description;
                existedServing.NumberOfPeople = updateServingViewModel.NumberOfPeople;
                existedServing.ProductId = updateServingViewModel.ProductId;
                existedServing.Name = updateServingViewModel.Name;

                _servingRepository.Modify(existedServing);
                await _uow.SaveAsync();

                return new ResponseViewModel { Success = true, Message = "Update successful" };
            }

            return new ResponseViewModel { Success = false, Message = "This serving does not exist" };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    private bool IsPriceDifferent(Models.Serving existedServing, UpdateServingViewModel updateServingViewModel)
    {
        return false;
        throw new NotImplementedException(); //TODO: finish later
    }

    public async Task<BaseListResponseViewModel> GetListServing(BaseListQueryParameters parameters)
    {
        try
        {
            var servings = _servingRepository.DbSet().AsNoTracking();

            return new BaseListResponseViewModel
            {
                Payload = await servings.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                .Select(q => new ServingViewModel()
                {
                    Description = q.Description,
                    Id = q.Id,
                    Name = q.Name,
                    NumberOfPeople = q.NumberOfPeople,
                    Price = q.Price,
                    ProductId = q.ProductId
                })
                .ToListAsync(),
                Success = true,
                Total = await servings.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            return new BaseListResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseListResponseViewModel> Search(BaseSearchParameters parameters)
    {
        try
        {
            var keyword = parameters.Keyword.ToLower();
            var servings = _servingRepository.DbSet().AsNoTracking()
                .Where(p => p.Name.ToLower().Contains(keyword));

            return new BaseListResponseViewModel
            {
                Payload = await servings.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                .Select(q => new ServingViewModel()
                {
                    Description = q.Description,
                    Id = q.Id,
                    Name = q.Name,
                    NumberOfPeople = q.NumberOfPeople,
                    Price = q.Price,
                    ProductId = q.ProductId
                })
                .ToListAsync(),
                Success = true,
                Total = await servings.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            return new BaseListResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<ResponseViewModel> GetServingById(Guid id)
    {
        try
        {
            var serving = await _servingRepository.GetById(id);
            var servingDetails = _mapper.Map<ServingViewDetailsModel>(serving);

            if (serving == null)
            {
                return new ResponseViewModel { Success = false, Message = "This serving does not exist" };
            }

            return new ResponseViewModel { Success = true, Payload = servingDetails };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }
}