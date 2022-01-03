using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Models.Repositories;
using LittleViet.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleViet.Data.Domains;

public interface IProductTypeDomain
{
    ResponseViewModel Create(CreateProductTypeViewModel productTypeVM);
    ResponseViewModel Update(UpdateProductTypeViewModel productTypeVM);
    ResponseViewModel Deactivate(Guid id);
    ResponseViewModel GetListProductType();
}
internal class ProductTypeDomain : BaseDomain, IProductTypeDomain
{
    private IProductTypeRepository _productTypeRepo;
    private readonly IMapper _mapper;
    public ProductTypeDomain(IUnitOfWork uow, IProductTypeRepository productTypeRepository, IMapper mapper) : base(uow)
    {
        _productTypeRepo = productTypeRepository;
        _mapper = mapper;
    }

    public ResponseViewModel Create(CreateProductTypeViewModel createProductTypeViewModel)
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
            _uow.Save();

            return new ResponseViewModel { Success = true, Message = "Create successful" };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public ResponseViewModel Update(UpdateProductTypeViewModel updateProductTypeViewModel)
    {
        try
        {
            var existedProType = _productTypeRepo.GetById(updateProductTypeViewModel.Id);

            if (existedProType != null)
            {
                existedProType.Name = updateProductTypeViewModel.Name;
                existedProType.Description = updateProductTypeViewModel.Description;
                existedProType.EsName = updateProductTypeViewModel.EsName;
                existedProType.CaName = updateProductTypeViewModel.CaName;
                existedProType.UpdatedDate = DateTime.UtcNow;
                existedProType.UpdatedBy = existedProType.UpdatedBy;

                _productTypeRepo.Modify(existedProType);
                _uow.Save();

                return new ResponseViewModel { Success = true, Message = "Update successful" };
            }

            return new ResponseViewModel { Success = false, Message = "This product type does not exist" };
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
            var productType = _productTypeRepo.GetById(id);
            _productTypeRepo.DeactivateProductType(productType);

            _uow.Save();
            return new ResponseViewModel { Message = "Delete successful", Success = true };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public ResponseViewModel GetListProductType()
    {
        try
        {
            var productTypes = _productTypeRepo.GetProductType();

            return new ResponseViewModel { Payload = productTypes, Success = true };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }
}

