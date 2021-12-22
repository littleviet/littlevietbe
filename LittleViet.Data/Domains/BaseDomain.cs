using LittleViet.Data.Models.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleViet.Data.Domains
{
    public partial class BaseDomain
    {
        protected IUnitOfWork _uow;
        public BaseDomain(IUnitOfWork uow)
        {
            _uow = uow;
        }
    }
}
