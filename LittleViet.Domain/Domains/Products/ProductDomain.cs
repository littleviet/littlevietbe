using AutoMapper;
using LittleViet.Data.Domains.ProductType;
using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Azure.AzureBlobStorage.Interface;
using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;
using LittleViet.Data.Models;
using LittleViet.Infrastructure.EntityFramework;
using static LittleViet.Infrastructure.EntityFramework.SqlHelper;

namespace LittleViet.Data.Domains.Products;
public interface IProductDomain
{
    Task<ResponseViewModel<Guid>> Create(CreateProductViewModel createProductViewModel);
    Task<ResponseViewModel> Update(UpdateProductViewModel updateProductViewModel);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListResponseViewModel> GetListProducts(GetListProductParameters parameters);
    Task<BaseListResponseViewModel> Search(BaseSearchParameters parameters);
    Task<ResponseViewModel<ProductDetailsViewModel>> GetProductById(Guid id);
    Task<ResponseViewModel> AddProductImages(AddProductImagesViewModel addProductImagesViewModel);
    Task<ResponseViewModel> DeactivateProductImage(Guid productId, Guid imageId);
    Task<ResponseViewModel> MakeMainProductImage(Guid productId, Guid imageId);
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

    public async Task<ResponseViewModel<Guid>> Create(CreateProductViewModel createProductViewModel)
    {
        
        var product = _mapper.Map<Models.Product>(createProductViewModel);
        var productId = Guid.NewGuid();
        product.Id = productId;

        await using var transaction = _uow.BeginTransaction();
        
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
            createStripeProductDto.Images = product.ProductImages.Where(pi => pi.IsMain).Select(p => p.Url).ToList();
            createStripeProductDto.Metadata = new () {{Infrastructure.Stripe.Payment.ProductMetaDataKey, productId.ToString()}};
            var stripeProduct = await _stripeProductService.CreateProduct(createStripeProductDto);
            product.StripeProductId = stripeProduct.Id;
            await _uow.SaveAsync();

            await transaction.CommitAsync();
            
            return new ResponseViewModel<Guid> { Success = true, Message = "Create successful", Payload = productId };
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
        await using var transaction = _uow.BeginTransaction();
        
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

            _productRepository.Modify(existedProduct);
            await _uow.SaveAsync();
            
            if (IsStripeProductChanged(existedProduct, updateProductViewModel))
            {
                var stripeProductDto = _mapper.Map<UpdateProductDto>(updateProductViewModel);
                stripeProductDto.Images = existedProduct.ProductImages.Where(pi => pi.IsMain).Select(p => p.Url).ToList();
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
        return product.Name != stripeProduct.Name || product.Description != stripeProduct.Description;
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
                    ContainsIgnoreCase<Models.Product>(new[] {nameof(Models.Product.Name), nameof(Models.Product.CaName), nameof(Models.Product.EsName)}, parameters.Name))
                .WhereIf(!string.IsNullOrEmpty(parameters.Description),
                    ContainsIgnoreCase<Models.Product>(nameof(Models.Product.Description), parameters.Description))
                .WhereIf(parameters.Statuses?.Any(), p => parameters.Statuses.Contains(p.Status))
                .WhereIf(parameters.ProductTypeId is not null, p => p.ProductTypeId == parameters.ProductTypeId);

            return new BaseListResponseViewModel
            {
                Payload = await products
                    .Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
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
                        Servings = p.Servings.Select(s => new GenericServingViewModel()
                        {
                            Id = s.Id,
                            Description = s.Description,
                            Name = s.Name,
                            Price = s.Price,
                            NumberOfPeople = s.NumberOfPeople,
                            ProductId = p.Id,
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

    public async Task<ResponseViewModel<ProductDetailsViewModel>> GetProductById(Guid id)
    {
        try
        {
            var product = await _productRepository.GetById(id);

            if (product == null)
                throw new Exception($"No product of Id {id} found");

            return new ResponseViewModel<ProductDetailsViewModel>()
            {
                Success = true,
                Payload = _mapper.Map<ProductDetailsViewModel>(product)
            };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> AddProductImages(AddProductImagesViewModel addProductImagesViewModel)
    {
        try
        {
            var imageUrls = await _blobProductImageService.CreateProductImages(addProductImagesViewModel.ProductImages);
            var productImages = imageUrls.Select(url => new ProductImage()
            {
                Url = url,
                ProductId = addProductImagesViewModel.ProductId,
                Id = Guid.NewGuid(),
                IsMain = false,
            }).ToList();
                
            _productImageRepository.AddRange(productImages);
            await _uow.SaveAsync();

            return new ResponseViewModel()
            {
                Success = true
            };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> DeactivateProductImage(Guid productId, Guid imageId)
    {
        try
        {
            var productImage = await _productImageRepository.DbSet().Where(pi => pi.Id == imageId).FirstOrDefaultAsync();
            if (productImage.ProductId != productId)
                throw new InvalidOperationException($"Product image of Id {imageId} does not belong to Product of Id {productId}");
            if (productImage.IsMain)
                throw new InvalidOperationException($"Product image of Id {imageId} cannot be removed because it is main Image of Product with Id {productId}");
            _productImageRepository.Deactivate(productImage);
            await _uow.SaveAsync();

            return new ResponseViewModel()
            {
                Success = true
            };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> MakeMainProductImage(Guid productId, Guid imageId)
    {
        await using var transaction = _uow.BeginTransaction();
        
        try
        {
            
            var product = await _productRepository.GetById(productId);
            
            foreach (var productImage in product.ProductImages)
            {
                productImage.IsMain = productImage.Id == imageId;
            }
            
            await _uow.SaveAsync();
            
            var stripeProductDto = new UpdateProductDto
            {
                Id = product.StripeProductId,
                Images = product.ProductImages.Where(pi => pi.IsMain).Select(p => p.Url).ToList(),
            };
            
            _ = await _stripeProductService.UpdateProduct(stripeProductDto);
            
            await transaction.CommitAsync();
            
            return new ResponseViewModel()
            {
                Success = true
            };
        }
        catch (Exception e)
        {
            throw;
        }
    }
}

