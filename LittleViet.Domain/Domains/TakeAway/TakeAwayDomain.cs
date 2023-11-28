using LittleViet.Domain.Repositories;
using LittleViet.Domain.ViewModels;
using LittleViet.Infrastructure.Caching;
using LittleViet.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Domain.Domains.TakeAway;

public interface ITakeAwayDomain
{
    Task<ResponseViewModel<List<GetListProductViewModel>>> GetMenuForTakeAway();
}

internal class TakeAwayDomain : BaseDomain, ITakeAwayDomain
{
    private readonly IProductRepository _productRepository;
    private readonly ICache _cache;

    public TakeAwayDomain(IUnitOfWork uow, IProductRepository productRepository, ICache cache) : base(uow)
    {
        _productRepository = productRepository;
        _cache = cache;
    }

    public async Task<ResponseViewModel<List<GetListProductViewModel>>> GetMenuForTakeAway()
    {
        var cacheResult =
            await _cache.TryGetAsync<ResponseViewModel<List<GetListProductViewModel>>>(CacheKeys
                .TakeAwayMenuCatalogCacheKey);

        if (cacheResult.Found)
            return cacheResult.Value;

        try
        {
            var products = _productRepository.DbSet()
                .Include(p => p.ProductType)
                .Include(p => p.Servings.Where(s => !s.IsDeleted))
                .Include(p => p.ProductImages.Where(pm => pm.IsMain))
                .Where(p => p.Servings.Any())
                .AsNoTracking();

            var payload = await products.Paginate(pageSize: 150, pageNum: 0)
                .Select(p => new GetListProductViewModel()
                {
                    Id = p.Id,
                    Description = p.Description,
                    Name = p.Name,
                    Status = p.Status,
                    CaName = p.CaName,
                    EsName = p.EsName,
                    ProductType = new GetListProductViewModel.GetListProductTypeViewModel()
                    {
                        Id = p.ProductType.Id,
                        Name = p.ProductType.Name,
                    },
                    Servings = p.Servings.Where(s => !s.IsDeleted).Select(s => new GenericServingViewModel()
                    {
                        Description = s.Description,
                        Id = s.Id,
                        Name = s.Name,
                        Price = s.Price,
                        NumberOfPeople = s.NumberOfPeople,
                        ProductId = p.Id
                    }).ToList(),
                    ImageUrl = p.ProductImages.Where(pm => pm.IsMain).Select(pm => pm.Url).SingleOrDefault(),
                }).OrderByDescending(p => p.ProductType.Name).ToListAsync();


            var result = new ResponseViewModel<List<GetListProductViewModel>>()
            {
                Payload = payload,
                Success = true,
                Message = $"Up to 100 products of total {await products.CountAsync()}",
            };
            await _cache.SetAsync(CacheKeys.TakeAwayMenuCatalogCacheKey, result);
            return result;
        }
        catch (Exception e)
        {
            return new() { Success = false, Message = e.Message };
        }
    }
}