using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Domains.TakeAway;

public interface ITakeAwayDomain
{
    Task<ResponseViewModel<GetListProductViewModel>> GetMenuForTakeAway();
}
internal class TakeAwayDomain : BaseDomain, ITakeAwayDomain
{
    private readonly IProductRepository _productRepository;

    public TakeAwayDomain(IUnitOfWork uow, IProductRepository productRepository) : base(uow)
    {
        _productRepository = productRepository;
    }

    public async Task<ResponseViewModel<GetListProductViewModel>> GetMenuForTakeAway()
    {
        try
        {
            var products = _productRepository.DbSet()
                .Include(p => p.ProductType)
                .Include(p => p.Servings.Where(s => !s.IsDeleted))
                .Include(p => p.ProductImages.Where(pm => pm.IsMain))
                .Where(p => p.Servings.Any())
                .AsNoTracking();

            return new BaseListResponseViewModel<GetListProductViewModel>
            {
                Payload = (await products.Paginate(pageSize: 50, pageNum: 0)
                    .Select(p => new GetListProductViewModel()
                    {
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
                        Servings = p.Servings.Select(s => new GenericServingViewModel()
                        {
                            Description = s.Description,
                            Id = s.Id,
                            Name = s.Name,
                            Price = s.Price,
                            NumberOfPeople = s.NumberOfPeople,
                        }).ToList(),
                        ImageUrl = p.ProductImages.Select(pm => pm.Url).FirstOrDefault(),
                    }).ToListAsync()),
                Success = true,
                Total = await products.CountAsync(),
                PageNumber = 0,
                PageSize = 50,
            };
        }
        catch (Exception e)
        {
            return new BaseListResponseViewModel<GetListProductViewModel> { Success = false, Message = e.Message };
        }
    }
}

