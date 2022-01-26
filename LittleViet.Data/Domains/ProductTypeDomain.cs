using AutoMapper;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Repositories;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using ProductType = LittleViet.Data.Models.ProductType;

namespace LittleViet.Data.Domains;

public interface IProductTypeDomain
{
    Task<ResponseViewModel> Create(CreateProductTypeViewModel createProductTypeViewModel);
    Task<ResponseViewModel> Update(UpdateProductTypeViewModel updateProductTypeViewModel);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListResponseViewModel<ProductTypeItemViewModel>> GetListProductTypes(BaseListQueryParameters parameters);
    Task<BaseListResponseViewModel> Search(BaseSearchParameters parameters);
    ResponseViewModel GetProductTypeById(Guid id);
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
            var productType = _mapper.Map<ProductType>(createProductTypeViewModel);

            var datetime = DateTime.UtcNow;

            productType.Id = Guid.NewGuid();
            productType.IsDeleted = false;
            productType.UpdatedDate = datetime;
            productType.CreatedDate = datetime;
            productType.UpdatedBy = createProductTypeViewModel.CreatedBy;

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
                existedProductType.UpdatedDate = DateTime.UtcNow;
                existedProductType.UpdatedBy = existedProductType.UpdatedBy;

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

    public ResponseViewModel GetProductTypeById(Guid id)
    {
        try
        {
            var productType = from pt in _productTypeRepository.DbSet()
                               .Include(t => t.Products.Where(p => p.IsDeleted == false))
                               .AsNoTracking()
                               .Where(q => q.Id == id)
                               .Take(1)
                              select new ProductTypeDetailsViewModel
                              {
                                  CaName = pt.CaName,
                                  EsName = pt.EsName,
                                  Name = pt.Name,
                                  Id = pt.Id,
                                  Description = pt.Description,
                                  Products = pt.Products.Select(p => new ProductViewModel
                                  {
                                      CaName = p.CaName,
                                      EsName = p.EsName,
                                      Name = p.Name,
                                      Description = p.Description,
                                      Status = p.Status,
                                      StatusName = p.Status.ToString()
                                  }).ToList()
                              };

            if (productType == null)
            {
                return new ResponseViewModel { Success = false, Message = "This product type does not exist" };
            }

            return new ResponseViewModel { Success = true, Payload = productType };
        }
        catch (Exception e)
        {
            throw;
        }
    }
}

