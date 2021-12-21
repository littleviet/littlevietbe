using LittleVietData.Models.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleVietData.Domains
{
    public interface IProductImageDomain
    {

    }
    internal class ProductImageDomain : BaseDomain, IProductImageDomain
    {
        public ProductImageDomain(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
