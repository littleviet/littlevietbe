using AutoMapper;
using LittleViet.Data.Domains.ProductType;
using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Domains.LandingPage;

public interface ILandingPageDomain
{
    Task<ResponseViewModel<LandingPageViewModel>> GetCatalogForLandingPage();
}

internal class LandingPageDomain : BaseDomain, ILandingPageDomain
{
    private readonly IProductTypeRepository _productTypeRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public LandingPageDomain(IUnitOfWork uow, IProductTypeRepository productTypeRepository,
        IProductRepository productRepository, IMapper mapper) : base(uow)
    {
        _productTypeRepository =
            productTypeRepository ?? throw new ArgumentNullException(nameof(productTypeRepository));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ResponseViewModel<LandingPageViewModel>> GetCatalogForLandingPage()
    {
        try
        {
            var productTypes = await _productTypeRepository.DbSet()
                    .Include(t => t.Products.Where(p => p.IsDeleted == false))
                    .ThenInclude(p => p.Servings)
                    .Where(pt => pt.Products.Any(p => p.Servings.Any()))
                    .AsNoTracking().ToListAsync();

            var packagedProducts = _productRepository.DbSet()
                .Where(x => x.ProductTypeId == Constants.PackagedProductTypeId);

            var result = new LandingPageViewModel()
            {
                MenuProducts =
                    productTypes
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
                    }).ToList(),
                PackagedProducts =
                    _mapper.Map<List<PackagedProductViewModel>>((await packagedProducts.ToListAsync()))
            };

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