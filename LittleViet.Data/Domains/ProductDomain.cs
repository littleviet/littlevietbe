using AutoMapper;
using Microsoft.Extensions.Configuration;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Repositories;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Azure.AzureBlobStorage.Interface;
using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Product = LittleViet.Data.Models.Product;
using ProductImage = LittleViet.Data.Models.ProductImage;

namespace LittleViet.Data.Domains;
public interface IProductDomain
{
    Task<ResponseViewModel> Create(Guid userId, CreateProductViewModel createProductViewModel);
    Task<ResponseViewModel> Update(Guid userId, UpdateProductViewModel updateProductViewModel);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListQueryResponseViewModel> GetListProducts(BaseListQueryParameters parameters);
    Task<BaseListQueryResponseViewModel> Search(BaseSearchParameters parameters);
    Task<ResponseViewModel> GetProductById(Guid id);
}

internal class ProductDomain : BaseDomain, IProductDomain
{
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _productImageRepository;
    private readonly IStripeProductService _stripeProductService;
    private readonly IMapper _mapper;
    private readonly IBlobService _blobService;
    private readonly IConfiguration _configuration;

    public ProductDomain(IUnitOfWork uow, IProductRepository productRepository, IProductImageRepository productImageRepository, IMapper mapper, IStripeProductService stripeProductService, IBlobService blobService, IConfiguration configuration) : base(uow)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _productImageRepository = productImageRepository ?? throw new ArgumentNullException(nameof(productImageRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _stripeProductService = stripeProductService ?? throw new ArgumentNullException(nameof(stripeProductService));
        _blobService = blobService ?? throw new ArgumentNullException(nameof(blobService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<ResponseViewModel> Create(Guid userId, CreateProductViewModel createProductViewModel)
    {
        try
        {
            var product = _mapper.Map<Product>(createProductViewModel);

            var productId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            product.Id = productId;
            product.IsDeleted = false;
            product.CreatedBy = userId;
            product.UpdatedDate = now;
            product.CreatedDate = now;
            product.UpdatedBy = userId;

            var productImages = createProductViewModel.ProductImages;
            var conn = _configuration["ConnectionStrings:LittleVietContainer"];
            var blobContainer = await _blobService.GetBlobContainer(conn, "products");

            for (var index = 0; index < productImages.Count; index++)
            {
                if (productImages[index].Image.Length > 0)
                {
                    string file_Extension = Path.GetExtension(productImages[index].Image.FileName);
                    string filename = Guid.NewGuid() + "" + (!string.IsNullOrEmpty(file_Extension) ? file_Extension : ".jpg");

                    await _blobService.UploadFileToBlobAsync(blobContainer, filename, productImages[index].Image.OpenReadStream());

                    var productImage = product.ProductImages.ElementAt(index);

                    productImage.Url = new Uri(blobContainer.Uri.AbsoluteUri) + "/" + filename;
                    productImage.ProductId = productId;
                    productImage.Id = Guid.NewGuid();
                    productImage.UpdatedBy = userId;
                    productImage.UpdatedDate = now;
                    productImage.CreatedBy = userId;
                    productImage.CreatedDate = now;
                    productImage.IsDeleted = false;
                }
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

    public async Task<ResponseViewModel> Update(Guid userId, UpdateProductViewModel updateProductViewModel)
    {
        try
        {
            var existedProduct = await _productRepository.DbSet().Include(q => q.ProductImages.Where(x => x.IsDeleted == false)).Where(q => q.Id == updateProductViewModel.Id).FirstOrDefaultAsync();

            if (existedProduct != null)
            {
                var stripeProductChanged = IsStripeProductChanged(existedProduct, updateProductViewModel);

                if (stripeProductChanged == true)
                {
                    var stripeProductDto = _mapper.Map<UpdateProductDto>(updateProductViewModel);
                    _ = await _stripeProductService.UpdateProduct(stripeProductDto);
                }

                var now = DateTime.UtcNow;

                existedProduct.Price = updateProductViewModel.Price;
                existedProduct.ProductTypeId = updateProductViewModel.ProductTypeId;
                existedProduct.Name = updateProductViewModel.Name;
                existedProduct.Description = updateProductViewModel.Description;
                existedProduct.EsName = updateProductViewModel.EsName;
                existedProduct.CaName = updateProductViewModel.CaName;
                existedProduct.Status = updateProductViewModel.Status;
                existedProduct.UpdatedDate = now;
                existedProduct.UpdatedBy = userId;

                if (updateProductViewModel.ImageChange)
                {
                    foreach (var item in existedProduct.ProductImages)
                    {
                        _productImageRepository.Deactivate(item);
                    }

                    var productImages = updateProductViewModel.ProductImages;
                    var conn = _configuration["ConnectionStrings:LittleVietContainer"];
                    var blobContainer = await _blobService.GetBlobContainer(conn, "products");

                    for (var index = 0; index < productImages.Count; index++)
                    {
                        if (productImages[index].Image.Length > 0)
                        {
                            string file_Extension = Path.GetExtension(productImages[index].Image.FileName);
                            string filename = Guid.NewGuid() + "" + (!string.IsNullOrEmpty(file_Extension) ? file_Extension : ".jpg");

                            await _blobService.UploadFileToBlobAsync(blobContainer, filename, productImages[index].Image.OpenReadStream());

                            existedProduct.ProductImages.Add(new ProductImage()
                            {
                                Id = Guid.NewGuid(),
                                Url = new Uri(blobContainer.Uri.AbsoluteUri) + "/" + filename,
                                ProductId = existedProduct.Id,
                                Name = productImages[index].Name,
                                IsDeleted = false,
                                IsMain = productImages[index].IsMain,
                                CreatedDate = now,
                                CreatedBy = userId,
                                UpdatedDate = now,
                                UpdatedBy = userId
                            });
                        }
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
}

