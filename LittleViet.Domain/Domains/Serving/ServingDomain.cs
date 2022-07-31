using AutoMapper;
using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.EntityFramework;
using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using static LittleViet.Infrastructure.EntityFramework.SqlHelper;
using Stripe;

namespace LittleViet.Data.Domains.Serving;

public interface IServingDomain
{
    Task<ResponseViewModel<Guid>> Create(CreateServingViewModel createServingViewModel);
    Task<ResponseViewModel> Update(UpdateServingViewModel updateServingViewModel);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListResponseViewModel> GetListServing(GetListServingParameters parameters);
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

    public async Task<ResponseViewModel<Guid>> Create(CreateServingViewModel createServingViewModel)
    {
        var serving = _mapper.Map<Models.Serving>(createServingViewModel);
        serving.Id = Guid.NewGuid();

        await using var transaction = _uow.BeginTransaction();
        try
        {
            var entry = _servingRepository.Add(serving);
            await _uow.SaveAsync();

            await entry.Reference(e => e.Product).LoadAsync();

            var createStripePriceDto = new CreatePriceDto()
            {
                Price = (long) serving.Price * 100,
                StripeProductId = entry.Entity.Product.StripeProductId,
                Metadata = new() {{Infrastructure.Stripe.Payment.ServingPriceMetaDataKey, serving.Id.ToString()}},
            };

            var stripePrice = await _stripePriceService.CreatePrice(createStripePriceDto);
            serving.StripePriceId = stripePrice.Id;

            await _uow.SaveAsync();
            await transaction.CommitAsync();

            return new ResponseViewModel<Guid> {Success = true, Message = "Create successful", Payload = serving.Id};
        }
        catch (StripeException se)
        {
            await transaction.RollbackAsync();
            Log.Warning("Stripe error when creating {servingId} with {exception}", serving.Id, se.ToString());
            throw;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Log.Warning("Error when creating {servingId} with {exception}", serving.Id, e.ToString());
            return new ResponseViewModel<Guid> {Success = false, Message = e.Message};
        }
    }

    public async Task<ResponseViewModel> Deactivate(Guid id)
    {
        var serving = await _servingRepository.GetById(id);
        await using var transaction = _uow.BeginTransaction();

        try
        {
            if (serving == null)
                return new ResponseViewModel {Success = false, Message = "This serving does not exist"};

            _servingRepository.Deactivate(serving);
            await _uow.SaveAsync();
            await _stripePriceService.DeactivatePrice(serving.StripePriceId);
            await transaction.CommitAsync();
            return new ResponseViewModel {Success = true, Message = "Delete successful"};
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return new ResponseViewModel {Success = false, Message = e.Message};
        }
    }

    public async Task<ResponseViewModel> Update(UpdateServingViewModel updateServingViewModel)
    {
        var existedServing = await _servingRepository.GetById(updateServingViewModel.Id); // TODO: check if okay
        if (existedServing == null)
            return new ResponseViewModel {Success = false, Message = "This serving does not exist"};

        await using var transaction = _uow.BeginTransaction();

        try
        {
            if (IsPriceDifferent(existedServing, updateServingViewModel))
            {
                var newPrice = await _stripePriceService.UpdatePrice(
                    new UpdatePriceDto
                    {
                        Amount = (long) (existedServing.Price * 100),
                        Currency = "eur",
                        Id = existedServing.StripePriceId,
                        ProductId = existedServing.Product.StripeProductId,
                        Metadata = new()
                            {{Infrastructure.Stripe.Payment.ServingPriceMetaDataKey, existedServing.Id.ToString()}},
                    });

                existedServing.StripePriceId = newPrice.Id;
            }

            existedServing.Price = updateServingViewModel.Price;
            existedServing.Description = updateServingViewModel.Description;
            existedServing.NumberOfPeople = updateServingViewModel.NumberOfPeople;
            existedServing.ProductId = updateServingViewModel.ProductId;
            existedServing.Name = updateServingViewModel.Name;

            _servingRepository.Modify(existedServing);
            await _uow.SaveAsync();
            await transaction.CommitAsync();

            return new ResponseViewModel {Success = true, Message = "Update successful"};
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return new ResponseViewModel {Success = false, Message = e.Message};
        }
    }

    private bool IsPriceDifferent(Models.Serving existedServing, UpdateServingViewModel updateServingViewModel)
    {
        return Math.Abs(existedServing.Price - updateServingViewModel.Price) >
               .00001f; //TODO: figure this float out later, also do full checks
    }

    public async Task<BaseListResponseViewModel> GetListServing(GetListServingParameters parameters)
    {
        try
        {
            var servings = _servingRepository.DbSet().AsNoTracking()
                .ApplySort(parameters.OrderBy)
                .WhereIf(!string.IsNullOrEmpty(parameters.Name),
                    ContainsIgnoreCase<Models.Serving>(nameof(Models.Serving.Name), parameters.Name))
                .WhereIf(!string.IsNullOrEmpty(parameters.Description),
                    ContainsIgnoreCase<Models.Serving>(nameof(Models.Serving.Description), parameters.Description))
                .WhereIf(parameters.NumberOfPeople is not null, s => s.NumberOfPeople == parameters.NumberOfPeople)
                .WhereIf(parameters.PriceFrom is not null, s => s.Price >= parameters.PriceFrom)
                .WhereIf(parameters.PriceTo is not null, s => s.Price <= parameters.PriceTo)
                .WhereIf(parameters.ProductId is not null, s => s.ProductId == parameters.ProductId);

            return new BaseListResponseViewModel
            {
                Payload = await servings.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                    .Select(q => new GenericServingViewModel()
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
            return new BaseListResponseViewModel {Success = false, Message = e.Message};
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
                Payload = await servings
                    .Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                    .ApplySort(parameters.OrderBy)
                    .Select(q => new GenericServingViewModel()
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
            return new BaseListResponseViewModel {Success = false, Message = e.Message};
        }
    }

    public async Task<ResponseViewModel> GetServingById(Guid id)
    {
        try
        {
            var serving = await _servingRepository.GetById(id);
            var servingDetails = _mapper.Map<ServingViewDetailsModel>(serving);

            return serving == null
                ? new ResponseViewModel {Success = false, Message = "This serving does not exist"}
                : new ResponseViewModel {Success = true, Payload = servingDetails};
        }
        catch (Exception e)
        {
            return new ResponseViewModel {Success = false, Message = e.Message};
        }
    }
}