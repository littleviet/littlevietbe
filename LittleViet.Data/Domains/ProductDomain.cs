using AutoMapper;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Repositories;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Product = LittleViet.Data.Models.Product;

namespace LittleViet.Data.Domains;

public interface IProductDomain
{
    Task<ResponseViewModel> Create(Guid userId, CreateProductViewModel createProductViewModel);
    Task<ResponseViewModel> Update(UpdateProductViewModel updateProductViewModel);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListQueryResponseViewModel> GetListProducts(BaseListQueryParameters parameters);
    Task<BaseListQueryResponseViewModel> Search(BaseSearchParameters parameters);
    Task<ResponseViewModel> GetProductById(Guid id);
}
internal class ProductDomain : BaseDomain, IProductDomain
{
    private readonly IProductRepository _productRepository;
    private readonly IStripeProductService _stripeProductService;
    private readonly IMapper _mapper;
    public ProductDomain(IUnitOfWork uow, IProductRepository productRepository, IMapper mapper, IStripeProductService stripeProductService) : base(uow)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _stripeProductService = stripeProductService ?? throw new ArgumentNullException(nameof(stripeProductService));
    }

    public async Task<ResponseViewModel> Create(Guid userId, CreateProductViewModel createProductViewModel)
    {
        try
        {
            var product = _mapper.Map<Product>(createProductViewModel);


            var datetime = DateTime.UtcNow;

            product.Id = Guid.NewGuid();
            product.IsDeleted = false;
            product.CreatedBy = userId;
            product.UpdatedDate = datetime;
            product.CreatedDate = datetime;
            product.UpdatedBy = userId;

            _productRepository.Add(product);
            await _uow.SaveAsync();

            var createStripeProductDto = _mapper.Map<CreateProductDto>(createProductViewModel);
            var stripeProduct = await _stripeProductService.CreateProduct(createStripeProductDto);
            product.StripeProductId = stripeProduct.Id;
            await _uow.SaveAsync();
            
            return new ResponseViewModel {Success = true, Message = "Create successful"};
        }
        catch (StripeException se)
        {
            throw;
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
                var stripeProductChanged = IsStripeProductChanged(existedProduct, updateProductViewModel);

                if (stripeProductChanged == true)
                {
                    var stripeProductDto = _mapper.Map<UpdateProductDto>(updateProductViewModel);
                    _ = await _stripeProductService.UpdateProduct(stripeProductDto);
                }
                
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

                return new ResponseViewModel {Success = true, Message = "Update successful"};
            }

            return new ResponseViewModel {Success = false, Message = "This product does not exist"};
        }
        catch (StripeException es)
        {
            throw;
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    private bool IsStripeProductChanged(Product product, UpdateProductViewModel stripeProduct)
    {
        // var imagesDifferent = product.ProductImages.Select(i => i.Url).ToList().Except(stripeProduct.); TODO: resolve this after finishing image upload
        return product.Name != stripeProduct.Name || product.Description != stripeProduct.Description;
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
                await _stripeProductService.DeactivateProduct(product.StripeProductId);
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
            throw;
        }
    }

    public ResponseViewModel GetProductsMenu()
    {
        try
        {
            var productsMenu = from pt in _productRepository.DbSet()
                    .Include(t => t.ProductType)
                    .AsNoTracking()
                    .AsEnumerable()
                select new ProductsMenuViewModel
                {
                    CaName = pt.CaName,
                    EsName = pt.EsName,
                    Name = pt.Name,
                    Description = pt.Description,
                    Id = pt.Id,
                    Price = pt.Price,
                    ProductTypeId = pt.ProductTypeId,
                    PropductType = pt.ProductType.Name,
                    EsPropductType = pt.ProductType.EsName,
                    CaPropductType = pt.CaName,
                    Status = pt.Status
                };

            return new ResponseViewModel { Payload = productsMenu.ToList(), Success = true };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }
}

