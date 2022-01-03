using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Models.Repositories;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains;

public interface IProductDomain
{
    ResponseViewModel Create(CreateProductViewModel productVm);
    ResponseViewModel Update(UpdateProductViewModel productVm);
    ResponseViewModel Deactivate(Guid id);
}
internal class ProductDomain : BaseDomain, IProductDomain
{
    private readonly IProductRepository _productRepo;
    private readonly IMapper _mapper;
    public ProductDomain(IUnitOfWork uow, IProductRepository productRepository, IMapper mapper) : base(uow)
    {
        _productRepo = productRepository;
        _mapper = mapper;
    }

    public ResponseViewModel Create(CreateProductViewModel createProductViewModel)
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

            _productRepo.Create(product);
            _uow.Save();

            return new ResponseViewModel { Success = true, Message = "Create successful" };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public ResponseViewModel Update(UpdateProductViewModel updateProductViewModel)
    {
        try
        {
            var existedPro = _productRepo.GetById(updateProductViewModel.Id);

            if (existedPro != null)
            {
                existedPro.Price = updateProductViewModel.Price;
                existedPro.ProductTypeId = updateProductViewModel.ProductTypeId;
                existedPro.Name = updateProductViewModel.Name;
                existedPro.Description = updateProductViewModel.Description;
                existedPro.EsName = updateProductViewModel.EsName;
                existedPro.CaName = updateProductViewModel.CaName;
                existedPro.Status = updateProductViewModel.Status;
                existedPro.UpdatedDate = DateTime.UtcNow;
                existedPro.UpdatedBy = updateProductViewModel.UpdatedBy;

                _productRepo.Modify(existedPro);
                _uow.Save();

                return new ResponseViewModel { Success = true, Message = "Update successful" };
            }

            return new ResponseViewModel { Success = false, Message = "This product does not exist" };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public ResponseViewModel Deactivate(Guid id)
    {
        try
        {
            var product = _productRepo.GetById(id);
            _productRepo.DeactivateProduct(product);

            _uow.Save();
            return new ResponseViewModel { Message = "Delete successful", Success = true };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }
}

