using LittleViet.Domain.Repositories;
using LittleViet.Domain.ViewModels;
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

    public TakeAwayDomain(IUnitOfWork uow, IProductRepository productRepository) : base(uow)
    {
        _productRepository = productRepository;
    }

    public async Task<ResponseViewModel<List<GetListProductViewModel>>> GetMenuForTakeAway()
    {
        try
        {
            var products = _productRepository.DbSet()
                .Include(p => p.ProductType)
                .Include(p => p.Servings.Where(s => !s.IsDeleted))
                .Include(p => p.ProductImages.Where(pm => pm.IsMain))
                .Where(p => p.Servings.Any())
                .AsNoTracking();
            
            return new()
            {
                Payload = (await products.Paginate(pageSize: 150, pageNum: 0)
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
                    }).OrderByDescending(p => p.ProductType.Name).ToListAsync()),
                Success = true,
                Message = $"Up to 100 products of total {await products.CountAsync()}",
            };
        }
        catch (Exception e)
        {
            return new() {Success = false, Message = e.Message};
        }
    }
}