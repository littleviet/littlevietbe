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
        ResponseVM Create(CreateProductTypeVM productTypeVM);
        ResponseVM Update(UpdateProductTypeVM productTypeVM);
        ResponseVM Deactivate(Guid id);
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

        public ResponseVM Create(CreateProductTypeVM productTypeVM)
        {
            try
            {
                var productType = _mapper.Map<ProductType>(productTypeVM);

                var datetime = DateTime.UtcNow;

                productType.Id = Guid.NewGuid();
                productType.IsDeleted = false;
                productType.UpdatedDate = datetime;
                productType.CreatedDate = datetime;
                productType.UpdatedBy = productTypeVM.CreatedBy;

                _productTypeRepo.Create(productType);
                _uow.Save();

                return new ResponseVM { Success = true, Message = "Create successful" };
            }
            catch (Exception e)
            {
                return new ResponseVM { Success = false, Message = e.Message };
            }
        }

        public ResponseVM Update(UpdateProductTypeVM productTypeVM)
        {
            try
            {
                var existedProType = _productTypeRepo.GetActiveById(productTypeVM.Id);

                if (existedProType != null)
                {
                    existedProType.Name = productTypeVM.Name;
                    existedProType.Description = productTypeVM.Description;
                    existedProType.ESName = productTypeVM.ESName;
                    existedProType.CAName = productTypeVM.CAName;
                    existedProType.UpdatedDate = DateTime.UtcNow;
                    existedProType.UpdatedBy = existedProType.UpdatedBy;

                    _productTypeRepo.Update(existedProType);
                    _uow.Save();

                    return new ResponseVM { Success = true, Message = "Update successful" };
                }

                return new ResponseVM { Success = false, Message = "This product type does not exist" };
            }
            catch (Exception e)
            {
                return new ResponseVM { Success = false, Message = e.Message };
            }
        }

        public ResponseVM Deactivate(Guid id)
        {
            try
            {
                var productType = _productTypeRepo.GetActiveById(id);
                _productTypeRepo.DeactivateProductType(productType);

                _uow.Save();
                return new ResponseVM { Message = "Delete successful", Success = true };
            }
            catch (Exception e)
            {
                return new ResponseVM { Success = false, Message = e.Message };
            }
        }
    }

