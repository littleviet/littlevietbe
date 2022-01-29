using AutoMapper;
using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Domains.ProductType;

public interface IProductTypeDomain
{
    Task<ResponseViewModel> Create(CreateProductTypeViewModel createProductTypeViewModel);
    Task<ResponseViewModel> Update(UpdateProductTypeViewModel updateProductTypeViewModel);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListResponseViewModel<ProductTypeItemViewModel>> GetListProductTypes(BaseListQueryParameters parameters);
    Task<BaseListResponseViewModel> Search(BaseSearchParameters parameters);
    Task<ResponseViewModel> GetProductTypeById(Guid id);
}
internal class ProductTypeDomain : BaseDomain, IProductTypeDomain
{
    private readonly IProductTypeRepository _productTypeRepository;
    private readonly IMapper _mapper;
    public ProductTypeDomain(IUnitOfWork uow, IProductTypeRepository productTypeRepository, IMapper mapper) : base(uow)
    {
        _productTypeRepository = productTypeRepository ?? throw new ArgumentNullException(nameof(productTypeRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ResponseViewModel> Create(CreateProductTypeViewModel createProductTypeViewModel)
    {
        try
        {
            var productType = _mapper.Map<Models.ProductType>(createProductTypeViewModel);

            productType.Id = Guid.NewGuid();

            _productTypeRepository.Add(productType);
            await _uow.SaveAsync();

            return new ResponseViewModel { Success = true, Message = "Create successful" };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> Update(UpdateProductTypeViewModel updateProductTypeViewModel)
    {
        try
        {
            var existedProductType = await _productTypeRepository.GetById(updateProductTypeViewModel.Id);

            if (existedProductType != null)
            {
                existedProductType.Name = updateProductTypeViewModel.Name;
                existedProductType.Description = updateProductTypeViewModel.Description;
                existedProductType.EsName = updateProductTypeViewModel.EsName;
                existedProductType.CaName = updateProductTypeViewModel.CaName;

                _productTypeRepository.Modify(existedProductType);
                await _uow.SaveAsync();

                return new ResponseViewModel { Success = true, Message = "Update successful" };
            }

            return new ResponseViewModel { Success = false, Message = "This product type does not exist" };
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
            var productType = await _productTypeRepository.GetById(id);

            if (productType != null)
            {
                _productTypeRepository.Deactivate(productType);

                await _uow.SaveAsync();
                return new ResponseViewModel { Message = "Delete successful", Success = true };
            }

            return new ResponseViewModel { Success = false, Message = "This product type does not exist" };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<BaseListResponseViewModel<ProductTypeItemViewModel>> GetListProductTypes(BaseListQueryParameters parameters)
    {
        try
        {
            var productTypes = _productTypeRepository.DbSet().AsNoTracking();

            return new BaseListResponseViewModel<ProductTypeItemViewModel>
            {
                Payload = await productTypes
                    .Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                    .Select(pt => new ProductTypeItemViewModel()
                    {
                        Id = pt.Id,
                        Name = pt.Name,
                        CaName = pt.CaName,
                        EsName = pt.EsName,
                        Description = pt.Description
                    })
                    .ToListAsync(),
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

    public async Task<BaseListResponseViewModel> Search(BaseSearchParameters parameters)
    {
        try
        {
            var keyword = parameters.Keyword.ToLower();
            var productTypes = _productTypeRepository.DbSet().AsNoTracking()
                .Where(p => p.Name.ToLower().Contains(keyword) || p.CaName.ToLower().Contains(keyword) || p.EsName.ToLower().Contains(keyword));

            return new BaseListResponseViewModel
            {
                Payload = await productTypes
                    .Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                    .Select(pt => new ProductTypeItemViewModel()
                    {
                        Id = pt.Id,
                        Name = pt.Name,
                        CaName = pt.CaName,
                        EsName = pt.EsName,
                        Description = pt.Description
                    })
                    .ToListAsync(),
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

    public async Task<ResponseViewModel> GetProductTypeById(Guid id)
    {
        try
        {
            var productType = await _productTypeRepository.DbSet()
                               .Include(t => t.Products.Where(p => p.IsDeleted == false))
                               .AsNoTracking()
                               .FirstOrDefaultAsync();
            
            if (productType == null)
            {
                return new ResponseViewModel { Success = false, Message = "This product type does not exist" };
            }
            
            var result = new ProductTypeDetailsViewModel
                  {
                      CaName = productType.CaName,
                      EsName = productType.EsName,
                      Name = productType.Name,
                      Id = productType.Id,
                      Description = productType.Description,
                      Products = productType.Products.Select(p => new ProductViewModel
                      {
                          CaName = p.CaName,
                          EsName = p.EsName,
                          Name = p.Name,
                          Description = p.Description,
                          Status = p.Status,
                          StatusName = p.Status.ToString()
                      }).ToList()
                  };
            
            return new ResponseViewModel { Success = true, Payload = result };
        }
        catch (Exception e)
        {
            throw;
        }
    }
}

