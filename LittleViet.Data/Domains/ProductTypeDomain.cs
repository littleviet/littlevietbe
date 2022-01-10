using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Models.Repositories;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace LittleViet.Data.Domains;

public interface IProductTypeDomain
{
    Task<ResponseViewModel> Create(CreateProductTypeViewModel createProductTypeViewModel);
    Task<ResponseViewModel> Update(UpdateProductTypeViewModel updateProductTypeViewModel);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListQueryResponseViewModel> GetListProductTypes(BaseListQueryParameters parameters);
    Task<BaseListQueryResponseViewModel> Search(BaseSearchParameters parameters);
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
            return new ResponseViewModel { Success = false, Message = e.Message };
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
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<ResponseViewModel> Deactivate(Guid id)
    {
        try
        {
            var productType = await _productTypeRepository.GetById(id);
            _productTypeRepository.Deactivate(productType);

            await _uow.SaveAsync();
            return new ResponseViewModel { Message = "Delete successful", Success = true };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseListQueryResponseViewModel> GetListProductTypes(BaseListQueryParameters parameters)
    {
        try
        {
            var productTypes = _productTypeRepository.DbSet().AsNoTracking();

            return new BaseListQueryResponseViewModel
            {
                Payload = await productTypes.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber).ToListAsync(),
                Success = true,
                Total = await productTypes.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            return new BaseListQueryResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseListQueryResponseViewModel> Search(BaseSearchParameters parameters)
    {
        try
        {
            var keyword = parameters.Keyword.ToLower();
            var productTypes = _productTypeRepository.DbSet().AsNoTracking()
                .Where(p => p.Name.ToLower().Contains(keyword) || p.CaName.ToLower().Contains(keyword) || p.EsName.ToLower().Contains(keyword));

            return new BaseListQueryResponseViewModel
            {
                Payload = await productTypes.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber).ToListAsync(),
                Success = true,
                Total = await productTypes.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            return new BaseListQueryResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<ResponseViewModel> GetProductTypeById(Guid id)
    {
        try
        {
            var productType = await _productTypeRepository.GetById(id);

            if (productType == null)
            {
                return new ResponseViewModel { Success = false, Message = "This product type does not exist" };
            }

            return new ResponseViewModel { Success = true, Payload = productType };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }
}

