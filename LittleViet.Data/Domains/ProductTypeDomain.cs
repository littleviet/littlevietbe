    using AutoMapper;
    using LittleViet.Data.Models;
    using LittleViet.Data.Models.Global;
    using LittleViet.Data.Models.Repositories;
    using LittleViet.Data.ViewModels;
    using LittleViet.Data.ServiceHelper;
    using Microsoft.EntityFrameworkCore;


namespace LittleViet.Data.Domains;

public interface IProductTypeDomain
{
    Task<ResponseViewModel> Create(CreateProductTypeViewModel productTypeVm);
    Task<ResponseViewModel> Update(UpdateProductTypeViewModel productTypeVm);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListQueryResponseViewModel> GetListProductType(BaseListQueryParameters parameters);
    Task<BaseListQueryResponseViewModel> Search(BaseSearchParameters parameters);
}
internal class ProductTypeDomain : BaseDomain, IProductTypeDomain
{
    private readonly IProductTypeRepository _productTypeRepo;
    private readonly IMapper _mapper;
    public ProductTypeDomain(IUnitOfWork uow, IProductTypeRepository productTypeRepository, IMapper mapper) : base(uow)
    {
        _productTypeRepo = productTypeRepository;
        _mapper = mapper;
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

            _productTypeRepo.Create(productType);
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
            var existedProType = await _productTypeRepo.GetById(updateProductTypeViewModel.Id);

            if (existedProType != null)
            {
                existedProType.Name = updateProductTypeViewModel.Name;
                existedProType.Description = updateProductTypeViewModel.Description;
                existedProType.EsName = updateProductTypeViewModel.EsName;
                existedProType.CaName = updateProductTypeViewModel.CaName;
                existedProType.UpdatedDate = DateTime.UtcNow;
                existedProType.UpdatedBy = existedProType.UpdatedBy;

                _productTypeRepo.Modify(existedProType);
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
            var productType = await _productTypeRepo.GetById(id);
            _productTypeRepo.DeactivateProductType(productType);

            await _uow.SaveAsync();
            return new ResponseViewModel { Message = "Delete successful", Success = true };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseListQueryResponseViewModel> GetListProductType(BaseListQueryParameters parameters)
    {
        try
        {
            var productTypes = _productTypeRepo.DbSet().AsNoTracking();

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
            var productTypes = _productTypeRepo.DbSet().AsNoTracking()
                .Where(p => p.Name.Contains(parameters.Keyword) || p.CaName.Contains(parameters.Keyword) || p.EsName.Contains(parameters.Keyword));

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
}

