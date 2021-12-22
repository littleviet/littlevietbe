using LittleViet.Data.Models.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleViet.Data.Domains
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
