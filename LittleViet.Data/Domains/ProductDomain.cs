using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Models.Repositories;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Domains;

public interface IProductDomain
{
    Task<ResponseViewModel> Create(CreateProductViewModel createProductViewModel);
    Task<ResponseViewModel> Update(UpdateProductViewModel updateProductViewModel);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListQueryResponseViewModel> GetListProducts(BaseListQueryParameters parameters);
    Task<BaseListQueryResponseViewModel> Search(BaseSearchParameters parameters);
    Task<ResponseViewModel> GetProductById(Guid id);
}
internal class ProductDomain : BaseDomain, IProductDomain
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    public ProductDomain(IUnitOfWork uow, IProductRepository productRepository, IMapper mapper) : base(uow)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ResponseViewModel> Create(CreateProductViewModel createProductViewModel)
    {
        try
        {
            var product = _mapper.Map<Product>(createProductViewModel);

            var datetime = DateTime.UtcNow;

            product.Id = Guid.NewGuid();
            product.IsDeleted = false;
            product.UpdatedDate = datetime;
            product.CreatedDate = datetime;
            product.UpdatedBy = createProductViewModel.CreatedBy;

            _productRepository.Add(product);
            await _uow.SaveAsync();

            return new ResponseViewModel { Success = true, Message = "Create successful" };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<ResponseViewModel> Update(UpdateProductViewModel updateProductViewModel)
    {
        try
        {
            var existedProduct = await _productRepository.GetById(updateProductViewModel.Id);

            if (existedProduct != null)
            {
                existedProduct.Price = updateProductViewModel.Price;
                existedProduct.ProductTypeId = updateProductViewModel.ProductTypeId;
                existedProduct.Name = updateProductViewModel.Name;
                existedProduct.Description = updateProductViewModel.Description;
                existedProduct.EsName = updateProductViewModel.EsName;
                existedProduct.CaName = updateProductViewModel.CaName;
                existedProduct.Status = updateProductViewModel.Status;
                existedProduct.UpdatedDate = DateTime.UtcNow;
                existedProduct.UpdatedBy = updateProductViewModel.UpdatedBy;

                _productRepository.Modify(existedProduct);
                await _uow.SaveAsync();

                return new ResponseViewModel { Success = true, Message = "Update successful" };
            }

            return new ResponseViewModel { Success = false, Message = "This product does not exist" };
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
            var product = await _productRepository.GetById(id);
            
            if(product != null)
            {
                _productRepository.Deactivate(product);

                await _uow.SaveAsync();
                return new ResponseViewModel { Message = "Delete successful", Success = true };
            }

            return new ResponseViewModel { Success = false, Message = "This product does not exist" };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseListQueryResponseViewModel> GetListProducts(BaseListQueryParameters parameters)
    {
        try
        {
            var products = _productRepository.DbSet().AsNoTracking();

            return new BaseListQueryResponseViewModel
            {
                Payload = await products.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber).ToListAsync(),
                Success = true,
                Total = await products.CountAsync(),
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
            var products = _productRepository.DbSet().AsNoTracking()
                .Where(p => p.Name.ToLower().Contains(keyword) || p.CaName.ToLower().Contains(keyword) || p.EsName.ToLower().Contains(keyword));

            return new BaseListQueryResponseViewModel
            {
                Payload = await products.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber).ToListAsync(),
                Success = true,
                Total = await products.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            return new BaseListQueryResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<ResponseViewModel> GetProductById(Guid id)
    {
        try
        {
            var product = await _productRepository.GetById(id);

            if (product == null)
            {
                return new ResponseViewModel { Success = false, Message = "This product does not exist" };
            }

            return new ResponseViewModel { Success = true, Payload = product };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }
}

