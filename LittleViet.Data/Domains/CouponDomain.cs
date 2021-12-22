using LittleViet.Data.Models.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleViet.Data.Domains
{
    public interface ICouponDomain
    {

    }
    internal class CouponDomain: BaseDomain, ICouponDomain
    {
        public CouponDomain(IUnitOfWork uow): base(uow)
        {

        }
    }
}
