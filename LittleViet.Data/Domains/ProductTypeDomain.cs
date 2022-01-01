using LittleViet.Data.Models.Global;
using LittleViet.Data.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleViet.Data.Domains
{
    public interface IProductTypeDomain
    {

    }
    internal class ProductTypeDomain:BaseDomain, IProductTypeDomain
    {
        private IProductTypeRepository _proTypeRepo;
        public ProductTypeDomain(IUnitOfWork uow, IProductTypeRepository productTypeRepository) : base(uow)
        {
            _proTypeRepo = productTypeRepository;
        }
    }
}
