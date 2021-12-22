using LittleViet.Data.Models.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleViet.Data.Domains
{
    public interface IProductDomain
    {

    }
    internal class ProductDomain : BaseDomain, IProductDomain
    {
        public ProductDomain(IUnitOfWork uow): base(uow)
        {

        }
    }
}
