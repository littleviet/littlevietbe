﻿using LittleViet.Data.Models.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleViet.Data.Domains
{
    public interface IServingDomain
    {

    }
    internal class ServingDomain : BaseDomain, IServingDomain
    {
        public ServingDomain(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
