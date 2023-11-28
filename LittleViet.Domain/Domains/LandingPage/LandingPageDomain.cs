using AutoMapper;
using LittleViet.Domain.Domains.ProductType;
using LittleViet.Domain.Repositories;
using LittleViet.Domain.ViewModels;
using LittleViet.Infrastructure.Caching;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Domain.Domains.LandingPage;

public interface ILandingPageDomain
{
    Task<ResponseViewModel<LandingPageViewModel>> GetCatalogForLandingPage();
}

internal class LandingPageDomain : BaseDomain, ILandingPageDomain
{
    private readonly IProductTypeRepository _productTypeRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICache _cache;

    public LandingPageDomain(IUnitOfWork uow, IProductTypeRepository productTypeRepository,
        IProductRepository productRepository, ICache cache) : base(uow)
    {
        _productTypeRepository =
            productTypeRepository ?? throw new ArgumentNullException(nameof(productTypeRepository));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _cache = cache;
    }

    public async Task<ResponseViewModel<LandingPageViewModel>> GetCatalogForLandingPage()
    {
        var cacheResult = await _cache.TryGetAsync<LandingPageViewModel>(CacheKeys.LandingPageCatalogCacheKey);
        if (cacheResult.Found)
            return new ResponseViewModel<LandingPageViewModel>
            {
                Payload = cacheResult.Value,
                Success = true,
            };

        try
        {
            var productTypes = _productTypeRepository.DbSet()
                .Include(t => t.Products.Where(p => p.IsDeleted))
                .ThenInclude(p => p.Servings)
                .Where(pt =>
                    pt.Products.Any(p => p.Servings.Any()) && pt.Id != ProductType.Constants.PackagedProductTypeId)
                .AsNoTracking();

            var packagedProducts = _productRepository
                .DbSet()
                .Include(x => x.ProductImages.Where(i => i.IsMain))
                .Where(x => x.ProductTypeId == ProductType.Constants.PackagedProductTypeId);

            var result = new LandingPageViewModel()
            {
                MenuProducts =
                    await productTypes
                        .Select(pt => new MenuProductTypeLandingPageViewModel
                        {
                            CaName = pt.CaName,
                            EsName = pt.EsName,
                            Name = pt.Name,
                            Products = pt.Products.Where(p => p.Servings.Any())
                                .Select(p => new MenuProductItemLandingPageViewModel
                                {
                                    CaName = p.CaName,
                                    EsName = p.EsName,
                                    Name = p.Name,
                                    Price = p.Servings.Min(s => s.Price),
                                }).ToList()
                        }).OrderByDescending(p => p.Name).ToListAsync(),
                PackagedProducts =
                    await packagedProducts
                        .Select(x =>
                            new PackagedProductViewModel()
                            {
                                Description = x.Description,
                                Id = x.Id,
                                Name = x.Name,
                                CaName = x.CaName,
                                EsName = x.EsName,
                                ImageUrl =
                                    x.ProductImages.Where(pm => pm.IsMain).Select(pm => pm.Url).SingleOrDefault(),
                            }
                        ).ToListAsync(),
            };

            await _cache.SetAsync(CacheKeys.LandingPageCatalogCacheKey, result);

            return new ResponseViewModel<LandingPageViewModel>
            {
                Payload = result,
                Success = true,
            };
        }
        catch (Exception e)
        {
            throw;
        }
    }
}