using AutoMapper;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Azure.AzureBlobStorage.Interface;
using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Models;
using LittleViet.Infrastructure.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using ProductImage = LittleViet.Data.Models.ProductImage;

namespace LittleViet.Data.Domains.Product;
public interface IProductDomain
{
    Task<ResponseViewModel> Create(Guid userId, CreateProductViewModel createProductViewModel, List<IFormFile> productImages);
    Task<ResponseViewModel> Update(Guid userId, UpdateProductViewModel updateProductViewModel, List<IFormFile> productImages);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListResponseViewModel> GetListProducts(BaseListQueryParameters parameters);
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
    private readonly IConfiguration _configuration;

    public ProductDomain(IUnitOfWork uow, IProductRepository productRepository, IProductImageRepository productImageRepository, IMapper mapper, IStripeProductService stripeProductService, IBlobProductImageService blobProductImageService, IConfiguration configuration) : base(uow)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _productImageRepository = productImageRepository ?? throw new ArgumentNullException(nameof(productImageRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _stripeProductService = stripeProductService ?? throw new ArgumentNullException(nameof(stripeProductService));
        _blobProductImageService = blobProductImageService ?? throw new ArgumentNullException(nameof(blobProductImageService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<ResponseViewModel> Create(Guid userId, CreateProductViewModel createProductViewModel, List<IFormFile> productImages)
    {
        try
        {
            var product = _mapper.Map<Models.Product>(createProductViewModel);

            var productId = Guid.NewGuid();

            product.Id = productId;

            if (productImages.Count > 0)
            {
                product.ProductImages = new List<ProductImage>();
                var imageLinks = await _blobProductImageService.CreateProductImages(productImages);

                product.ProductImages = imageLinks.Select((q, index) => new ProductImage()
                {
                    Url = imageLinks[index],
                    ProductId = productId,
                    Id = Guid.NewGuid(),
                    IsMain = createProductViewModel.MainImage == index ? true : false
                }).ToList();
            }

            _productRepository.Add(product);
            await _uow.SaveAsync();

            var createStripeProductDto = _mapper.Map<CreateProductDto>(createProductViewModel);
            var stripeProduct = await _stripeProductService.CreateProduct(createStripeProductDto);
            product.StripeProductId = stripeProduct.Id;
            await _uow.SaveAsync();

            return new ResponseViewModel { Success = true, Message = "Create successful" };
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

    public async Task<ResponseViewModel> Update(Guid userId, UpdateProductViewModel updateProductViewModel, List<IFormFile> images)
    {
        try
        {
            var existedProduct = await _productRepository.DbSet()
                .Include(q => q.ProductImages.Where(x => x.IsDeleted == false))
                .Where(q => q.Id == updateProductViewModel.Id)
                .FirstOrDefaultAsync();

            if (existedProduct != null)
            {
                if (IsStripeProductChanged(existedProduct, updateProductViewModel))
                {
                    var stripeProductDto = _mapper.Map<UpdateProductDto>(updateProductViewModel);
                    _ = await _stripeProductService.UpdateProduct(stripeProductDto);
                }

                existedProduct.ProductTypeId = updateProductViewModel.ProductTypeId;
                existedProduct.Name = updateProductViewModel.Name;
                existedProduct.Description = updateProductViewModel.Description;
                existedProduct.EsName = updateProductViewModel.EsName;
                existedProduct.CaName = updateProductViewModel.CaName;
                existedProduct.Status = updateProductViewModel.Status;

                if (updateProductViewModel.ImageChange)
                {
                    foreach (var item in existedProduct.ProductImages)
                    {
                        _productImageRepository.Deactivate(item);
                    }

                    if (images.Count > 0)
                    {
                        var imageLinks = await _blobProductImageService.CreateProductImages(images);

                        var productImages = imageLinks.Select((q, index) => new ProductImage()
                        {
                            Url = imageLinks[index],
                            ProductId = existedProduct.Id,
                            Id = Guid.NewGuid(),
                            IsMain = updateProductViewModel.MainImage == index ? true : false
                        }).ToList();

                        _productImageRepository.AddRange(productImages);
                    }
                }

                _productRepository.Modify(existedProduct);
                await _uow.SaveAsync();

                return new ResponseViewModel { Success = true, Message = "Update successful" };
            }

            return new ResponseViewModel { Success = false, Message = "This product does not exist" };
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

    private bool IsStripeProductChanged(Models.Product product, UpdateProductViewModel stripeProduct)
    {
        // var imagesDifferent = product.ProductImages.Select(i => i.Url).ToList().Except(stripeProduct.); TODO: resolve this after finishing image upload
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

    public async Task<BaseListResponseViewModel> GetListProducts(BaseListQueryParameters parameters)
    {
        try
        {
            var products = _productRepository.DbSet()
                .Include(p => p.ProductType)
                .Include(p => p.Servings)
                .Include(p => p.ProductImages.Where(pm => pm.IsMain))
                .AsNoTracking();

            return new BaseListResponseViewModel
            {
                Payload = await products.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
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

