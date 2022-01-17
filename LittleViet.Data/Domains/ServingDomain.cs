using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Repositories;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Models;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Domains;

public interface IServingDomain
{
    Task<ResponseViewModel> Create(CreateServingViewModel createServingViewModel);
    Task<ResponseViewModel> Update(UpdateServingViewModel updateServingViewModel);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListQueryResponseViewModel> GetListServing(BaseListQueryParameters parameters);
    Task<BaseListQueryResponseViewModel> Search(BaseSearchParameters parameters);
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
            var serving = _mapper.Map<Serving>(createServingViewModel);

            var date = DateTime.UtcNow;

            serving.Id = Guid.NewGuid();
            serving.CreatedDate = date;
            serving.UpdatedDate = date;
            serving.UpdatedBy = createServingViewModel.CreatedBy;
            serving.IsDeleted = false;

            _servingRepository.Add(serving);
            await _uow.SaveAsync();

            var createStripePriceDto = _mapper.Map<CreatePriceDto>(createServingViewModel);
            var stripePrice = await _stripePriceService.CreatePrice(createStripePriceDto);
            serving.StripePriceId = stripePrice.Id;

            await _uow.SaveAsync();

            return new ResponseViewModel {Success = true, Message = "Create successful"};
        }
        catch (Exception e)
        {
            return new ResponseViewModel {Success = false, Message = e.Message};
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

                return new ResponseViewModel {Success = true, Message = "Delete successful"};
            }

            return new ResponseViewModel {Success = false, Message = "This serving does not exist"};
        }
        catch (Exception e)
        {
            return new ResponseViewModel {Success = false, Message = e.Message};
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
                        Amount = existedServing.Price as long?,
                        
                    });
                existedServing.StripePriceId = newPrice.Id;
            }

            if (existedServing != null)
            {
                existedServing.UpdatedDate = DateTime.UtcNow;
                existedServing.Price = updateServingViewModel.Price;
                existedServing.Description = updateServingViewModel.Description;
                existedServing.NumberOfPeople = updateServingViewModel.NumberOfPeople;
                existedServing.ProductId = updateServingViewModel.ProductId;
                existedServing.Name = updateServingViewModel.Name;
                existedServing.UpdatedBy = updateServingViewModel.UpdatedBy;

                _servingRepository.Modify(existedServing);
                await _uow.SaveAsync();

                return new ResponseViewModel {Success = true, Message = "Update successful"};
            }

            return new ResponseViewModel {Success = false, Message = "This serving does not exist"};
        }
        catch (Exception e)
        {
            return new ResponseViewModel {Success = false, Message = e.Message};
        }
    }

    private bool IsPriceDifferent(Serving existedServing, UpdateServingViewModel updateServingViewModel)
    {
        return false;
        throw new NotImplementedException(); //TODO: finish later
    }

    public async Task<BaseListQueryResponseViewModel> GetListServing(BaseListQueryParameters parameters)
    {
        try
        {
            var servings = _servingRepository.DbSet().AsNoTracking();

            return new BaseListQueryResponseViewModel
            {
                Payload = await servings.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                    .ToListAsync(),
                Success = true,
                Total = await servings.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            return new BaseListQueryResponseViewModel {Success = false, Message = e.Message};
        }
    }

    public async Task<BaseListQueryResponseViewModel> Search(BaseSearchParameters parameters)
    {
        try
        {
            var keyword = parameters.Keyword.ToLower();
            var servings = _servingRepository.DbSet().AsNoTracking()
                .Where(p => p.Name.ToLower().Contains(keyword));

            return new BaseListQueryResponseViewModel
            {
                Payload = await servings.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                    .ToListAsync(),
                Success = true,
                Total = await servings.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            return new BaseListQueryResponseViewModel {Success = false, Message = e.Message};
        }
    }

    public async Task<ResponseViewModel> GetServingById(Guid id)
    {
        try
        {
            var serving = await _servingRepository.GetById(id);

            if (serving == null)
            {
                return new ResponseViewModel {Success = false, Message = "This serving does not exist"};
            }

            return new ResponseViewModel {Success = true, Payload = serving};
        }
        catch (Exception e)
        {
            return new ResponseViewModel {Success = false, Message = e.Message};
        }
    }
}