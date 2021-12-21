using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleVietData.Models.Global
{
    public partial interface IUnitOfWork
    {
        T GetService<T>();
        int SaveChanges();
        IDbContextTransaction BeginTransation();
    }

    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IServiceProvider service, LittleVietContext context)
        {
            this.service = service;
            this.context = context;
        }

        protected readonly IServiceProvider service;
        protected readonly LittleVietContext context;

        public IDbContextTransaction BeginTransation()
        {
            throw new NotImplementedException();
        }

        public T GetService<T>()
        {
            return service.GetService<T>();
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }
    }
}
