using AutoMapper;
using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Azure.AzureBlobStorage.Interface;
using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;
using LittleViet.Data.Models;
using static LittleViet.Infrastructure.Mvc.SqlHelper;
using LittleViet.Infrastructure.Mvc;

namespace LittleViet.Data.Domains.Products;
public interface IProductDomain
{
    Task<ResponseViewModel> Create(CreateProductViewModel createProductViewModel);
    Task<ResponseViewModel> Update(UpdateProductViewModel updateProductViewModel);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListResponseViewModel> GetListProducts(GetListProductParameters parameters);
    Task<BaseListResponseViewModel> Search(BaseSearchParameters parameters);
    Task<ResponseViewModel> GetProductById(Guid id);
}

internal class ProductDomain : BaseDomain, IProductDomain
{
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _productImageRepository;
    private readonly IStripeProductService _stripeProductService;
    private readonly IMapper _mapper;
    private readonly IBlobProductImageService _blobProductImageService;

    public ProductDomain(IUnitOfWork uow, IProductRepository productRepository, IProductImageRepository productImageRepository, IMapper mapper, IStripeProductService stripeProductService, IBlobProductImageService blobProductImageService) : base(uow)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _productImageRepository = productImageRepository ?? throw new ArgumentNullException(nameof(productImageRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _stripeProductService = stripeProductService ?? throw new ArgumentNullException(nameof(stripeProductService));
        _blobProductImageService = blobProductImageService ?? throw new ArgumentNullException(nameof(blobProductImageService));
    }

    public async Task<ResponseViewModel> Create(CreateProductViewModel createProductViewModel)
    {
        
        var product = _mapper.Map<Models.Product>(createProductViewModel);
        var productId = Guid.NewGuid();
        product.Id = productId;

        await using var transaction = _uow.BeginTransation();
        
        try
        {
            if (createProductViewModel.ProductImages.Any())
            {
                product.ProductImages = new List<ProductImage>();
                var imageUrls = await _blobProductImageService.CreateProductImages(createProductViewModel.ProductImages);
            
                product.ProductImages = imageUrls.Select((q, index) => new ProductImage()
                {
                    Url = imageUrls[index],
                    ProductId = productId,
                    Id = Guid.NewGuid(),
                    IsMain = createProductViewModel.MainImage == index
                }).ToList();
            }

            _productRepository.Add(product);
            await _uow.SaveAsync();

            var createStripeProductDto = _mapper.Map<CreateProductDto>(createProductViewModel);
            createStripeProductDto.Images = product.ProductImages.Select(p => p.Url).ToList();
            createStripeProductDto.Metadata = new () {{Infrastructure.Stripe.Payment.ProductMetaDataKey, productId.ToString()}};
            var stripeProduct = await _stripeProductService.CreateProduct(createStripeProductDto);
            product.StripeProductId = stripeProduct.Id;
            await _uow.SaveAsync();

            await transaction.CommitAsync();
            
            return new ResponseViewModel { Success = true, Message = "Create successful" };
        }
        catch (StripeException se)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ResponseViewModel> Update(UpdateProductViewModel updateProductViewModel)
    {
        await using var transaction = _uow.BeginTransation();
        
        try
        {
            var existedProduct = await _productRepository.DbSet()
                .Include(q => q.ProductImages.Where(x => x.IsDeleted == false))
                .Where(q => q.Id == updateProductViewModel.Id)
                .FirstOrDefaultAsync();

            if (existedProduct == null)
                return new ResponseViewModel {Success = false, Message = "This product does not exist"};

            existedProduct.ProductTypeId = updateProductViewModel.ProductTypeId;
            existedProduct.Name = updateProductViewModel.Name;
            existedProduct.Description = updateProductViewModel.Description;
            existedProduct.EsName = updateProductViewModel.EsName;
            existedProduct.CaName = updateProductViewModel.CaName;
            existedProduct.Status = updateProductViewModel.Status;

            if (updateProductViewModel.ImageChange)
            {
                foreach (var item in existedProduct.ProductImages)
                    _productImageRepository.Deactivate(item);

                if (updateProductViewModel.ProductImages.Any())
                {
                    var imageLinks = await _blobProductImageService.CreateProductImages(updateProductViewModel.ProductImages);

                    var productImages = imageLinks.Select((q, index) => new ProductImage()
                    {
                        Url = imageLinks[index],
                        ProductId = existedProduct.Id,
                        Id = Guid.NewGuid(),
                        IsMain = updateProductViewModel.MainImage == index
                    }).ToList();

                    _productImageRepository.AddRange(productImages);
                }
            }

            _productRepository.Modify(existedProduct);
            await _uow.SaveAsync();
            
            if (IsStripeProductChanged(existedProduct, updateProductViewModel) || updateProductViewModel.ImageChange)
            {
                var stripeProductDto = _mapper.Map<UpdateProductDto>(updateProductViewModel);
                stripeProductDto.Images = existedProduct.ProductImages.Select(p => p.Url).ToList();
                stripeProductDto.Metadata = new () {{Infrastructure.Stripe.Payment.ProductMetaDataKey, existedProduct.Id.ToString()}};
                _ = await _stripeProductService.UpdateProduct(stripeProductDto);
            }
            
            await transaction.CommitAsync();
            
            return new ResponseViewModel { Success = true, Message = "Update successful" };
        }
        catch (StripeException es)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (Exception e)
        {            
            await transaction.RollbackAsync();
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    private bool IsStripeProductChanged(Models.Product product, UpdateProductViewModel stripeProduct)
    {
        // var imagesDifferent = product.ProductImages.Select(i => i.Url).ToList().Except(stripeProduct.); TODO: use better logic for this
        return product.Name != stripeProduct.Name || product.Description != stripeProduct.Description || stripeProduct.ImageChange;
    }

    public async Task<ResponseViewModel> Deactivate(Guid id)
    {
        try
        {
            var product = await _productRepository.GetById(id);

            if (product != null)
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

    public async Task<BaseListResponseViewModel> GetListProducts(GetListProductParameters parameters)
    {
        try
        {
            var products = _productRepository.DbSet().AsNoTracking()
                .ApplySort(parameters.OrderBy)
                .Include(p => p.ProductType)
                .Include(p => p.Servings)
                .Include(p => p.ProductImages.Where(pm => pm.IsMain))
                .WhereIf(!string.IsNullOrEmpty(parameters.Name),
                    ContainsIgnoreCase<Models.Product>(nameof(Models.Product.Name), parameters.Name))
                .WhereIf(!string.IsNullOrEmpty(parameters.CaName),
                    ContainsIgnoreCase<Models.Product>(nameof(Models.Product.CaName), parameters.CaName))
                .WhereIf(!string.IsNullOrEmpty(parameters.EsName),
                    ContainsIgnoreCase<Models.Product>(nameof(Models.Product.EsName), parameters.EsName))
                .WhereIf(!string.IsNullOrEmpty(parameters.Description),
                    ContainsIgnoreCase<Models.Product>(nameof(Models.Product.Description), parameters.Description))
                .WhereIf(parameters.Statuses is not null && parameters.Statuses.Any(), p => parameters.Statuses.Contains(p.Status))
                .WhereIf(parameters.ProductTypeId is not null, p => p.ProductTypeId == parameters.ProductTypeId);

            return new BaseListResponseViewModel
            {
                Payload = await products
                    .Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
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
                        Servings = p.Servings.Select(s => new ServingViewModel()
                        {
                            Description = s.Description,
                            Id = s.Id,
                            Name = s.Name,
                            Price = s.Price,
                            NumberOfPeople = s.NumberOfPeople,
                        }).ToList(),
                        ImageUrl = p.ProductImages.Select(pm => pm.Url).FirstOrDefault(),
                    }).ToListAsync(),
                Success = true,
                Total = await products.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            return new BaseListResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseListResponseViewModel> Search(BaseSearchParameters parameters)
    {
        try
        {
            var keyword = parameters.Keyword.ToLower();
            var products = _productRepository.DbSet().AsNoTracking()
                .Where(p => p.Name.ToLower().Contains(keyword) || p.CaName.ToLower().Contains(keyword) || p.EsName.ToLower().Contains(keyword));

            return new BaseListResponseViewModel
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
            return new BaseListResponseViewModel { Success = false, Message = e.Message };
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
}

