using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.ViewModels;

    internal class ProductLPVM
    {
        public string ProductType { get; set; }
        public ProductsLP Products { get; set; }
    }

    internal class ProductsLP
    {
        public string Name { get; set; }
        public string ESName { get; set; }
        public string CAName { get; set; }
        public double Price { get; set; }
    }

    public class CreateProductVM
    {
        public Guid CreatedBy { get; set; }
        public string Name { get; set; }
        public string ESName { get; set; }
        public string CAName { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public ProductSatus Status { get; set; }
        public Guid ProductTypeId { get; set; }
    }

    public class UpdateProductVM
    {
        public Guid UpdatedBy { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ESName { get; set; }
        public string CAName { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public ProductSatus Status { get; set; }
        public Guid ProductTypeId { get; set; }
    }

