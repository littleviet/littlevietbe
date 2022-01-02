using LittleViet.Data.ServiceHelper;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LittleViet.Data.Models;

    public class Product : AuditableEntity
    {
        public Product()
        {

        }

        public string Name { get; set; }
        public string ESName { get; set; }
        public string CAName { get; set; }
        public string Description { get; set; }
        public Guid ProductTypeId { get; set; }
        public ProductSatus Status { get; set; }
        public double Price { get; set; }

        public virtual ProductType ProductType { get; set; }
}

