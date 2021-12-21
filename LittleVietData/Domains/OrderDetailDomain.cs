using LittleVietData.Models.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleVietData.Domains
{
    public interface IOrderDeitalDomain
    {

    }
    internal class OrderDetailDomain: BaseDomain, IOrderDeitalDomain
    {
        public OrderDetailDomain(IUnitOfWork uow): base(uow)
        {

        }
    }
}
