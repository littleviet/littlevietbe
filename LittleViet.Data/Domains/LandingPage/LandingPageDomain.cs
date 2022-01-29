using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Domains.LandingPage;

public interface ILandingPageDomain
{
    ResponseViewModel GetCatalogForLandingPage();
}
internal class LandingPageDomain : BaseDomain, ILandingPageDomain
{
    private readonly IProductTypeRepository _productTypeRepository;

    public LandingPageDomain(IUnitOfWork uow, IProductTypeRepository productTypeRepository) : base(uow)
    {
        _productTypeRepository = productTypeRepository;
    }

    public ResponseViewModel GetCatalogForLandingPage()
    {
        try
        {
            var productTypes = from pt in _productTypeRepository.DbSet()
                               .Include(t => t.Products.Where(p => p.IsDeleted == false))
                                 .ThenInclude(p => p.Servings)
                               .AsNoTracking()
                               .AsEnumerable()
                               select new ProductLandingPageViewModel
                               {
                                   CaName = pt.CaName,
                                   EsName = pt.EsName,
                                   Name = pt.Name,
                                   Products = pt.Products.Select(p => new ProductsLandingPageViewModel
                                   {
                                       CaName = p.CaName,
                                       EsName = p.EsName,
                                       Name = p.Name,
                                       Price = p.Servings.Any() ? p.Servings.Min(s => s.Price) : null,
                                   }).ToList()
                               };

            return new ResponseViewModel { Payload = productTypes.ToList(), Success = true };
        }
        catch (Exception e)
        {
            throw;
        }
    }
}

