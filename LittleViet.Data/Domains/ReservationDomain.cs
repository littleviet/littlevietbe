using LittleViet.Data.Models.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleViet.Data.Domains
{
    public interface IReservationDomain
    {

    }
    internal class ReservationDomain : BaseDomain, IReservationDomain
    {
        public ReservationDomain(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
