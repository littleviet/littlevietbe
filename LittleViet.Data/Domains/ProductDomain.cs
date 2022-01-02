using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Models.Repositories;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains;

    public interface IProductDomain
    {
        ResponseVM Create(CreateProductVM productVM);
        ResponseVM Update(UpdateProductVM productVM);
        ResponseVM Deactivate(Guid id);
        ResponseVM GetActivesForLP();
    }
    internal class ProductDomain : BaseDomain, IProductDomain
    {
        private IProductRepository _productRepo;
        private readonly IMapper _mapper;
        public ProductDomain(IUnitOfWork uow, IProductRepository productRepository, IMapper mapper) : base(uow)
        {
            _productRepo = productRepository;
            _mapper = mapper;
        }

        public ResponseVM Create(CreateProductVM productVM)
        {
            try
            {
                var product = _mapper.Map<Product>(productVM);

                var datetime = DateTime.UtcNow;

                product.Id = Guid.NewGuid();
                product.IsDeleted = false;
                product.UpdatedDate = datetime;
                product.CreatedDate = datetime;
                product.UpdatedBy = productVM.CreatedBy;

                _productRepo.Create(product);
                _uow.Save();

                return new ResponseVM { Success = true, Message = "Create successful" };
            }
            catch (Exception e)
            {
                return new ResponseVM { Success = false, Message = e.Message };
            }
        }

        public ResponseVM Update(UpdateProductVM productVM)
        {
            try
            {
                var existedPro = _productRepo.GetActiveById(productVM.Id);

                if (existedPro != null)
                {
                    existedPro.Price = productVM.Price;
                    existedPro.ProductTypeId = productVM.ProductTypeId;
                    existedPro.Name = productVM.Name;
                    existedPro.Description = productVM.Description;
                    existedPro.ESName = productVM.ESName;
                    existedPro.CAName = productVM.CAName;
                    existedPro.Status = productVM.Status;
                    existedPro.UpdatedDate = DateTime.UtcNow;
                    existedPro.UpdatedBy = productVM.UpdatedBy;

                    _productRepo.Update(existedPro);
                    _uow.Save();

                    return new ResponseVM { Success = true, Message = "Update successful" };
                }

                return new ResponseVM { Success = false, Message = "This product does not exist" };
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
                var product = _productRepo.GetActiveById(id);
                _productRepo.DeactivateProduct(product);

                _uow.Save();
                return new ResponseVM { Message = "Delete successful", Success = true };
            }
            catch (Exception e)
            {
                return new ResponseVM { Success = false, Message = e.Message };
            }
        }

        public ResponseVM GetActivesForLP()
        {
            try
            {
                var products = _productRepo.GetActiveProducs();
                var result = new List<ProductLPVM>();

                foreach (var item in products)
                {
                    var pro = _mapper.Map<ProductsLP>(item);
                    result.Add(new ProductLPVM
                    {
                        Products = pro,
                        ProductType = item.ProductType.Name
                    });
                }

                return new ResponseVM { Payload = result, Success = true };
            }
            catch (Exception e)
            {
                return new ResponseVM { Success = false, Message = e.Message };
            }
        }
    }

