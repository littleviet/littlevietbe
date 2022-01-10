using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.ViewModels;

public class CreateProductViewModel
{
    public Guid CreatedBy { get; set; }
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public ProductSatus Status { get; set; }
    public Guid ProductTypeId { get; set; }
}

public class UpdateProductViewModel
{
    public Guid UpdatedBy { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public ProductSatus Status { get; set; }
    public Guid ProductTypeId { get; set; }
}

public class ProductViewModel
{
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public ProductSatus Status { get; set; }
    public string ProductTypeName { get; set; }
}

public class ProductDetailsViewModel
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? CreatedDate { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid UpdatedBy { get; set; }
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public ProductSatus Status { get; set; }
    public double Price { get; set; }
    public Guid ProductTypeId { get; set; }
    public string ProductTypeName { get; set; }
}