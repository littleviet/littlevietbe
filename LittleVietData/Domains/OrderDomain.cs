﻿using LittleVietData.Models.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleVietData.Domains
{
    public interface IOrderDomain
    {

    }
    internal class OrderDomain : BaseDomain, IOrderDomain
    {
        public OrderDomain(IUnitOfWork uow): base(uow)
        {

        }
    }
}
