using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.ViewModels;

public class CreateProductViewModel
{
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public ProductStatus Status { get; set; }
    public Guid ProductTypeId { get; set; }
    public int MainImage { get; set; }
}

public class UpdateProductViewModel
{
    public Guid UpdatedBy { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public ProductStatus Status { get; set; }
    public Guid ProductTypeId { get; set; }
    public bool ImageChange { get; set; }
    public int MainImage { get; set; }
}

public class ProductViewModel
{
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public ProductStatus Status { get; set; }
    public string StatusName { get; set; }
    public string ProductTypeName { get; set; }
}

public class GetListProductViewModel
{
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public List<ServingViewModel> Servings { get; set; }
    public ProductStatus Status { get; set; }
    public string ImageUrl { get; set; }    
    public GetListProductTypeViewModel ProductType { get; set; }

    public class GetListProductTypeViewModel
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
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
    public ProductStatus Status { get; set; }
    public Guid ProductTypeId { get; set; }
    public string ProductTypeName { get; set; }
}

public class ProductsMenuViewModel
{
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public ProductStatus Status { get; set; }
    public Guid Id { get; set; }
    public Guid ProductTypeId { get; set; }
    public string PropductType { get; set; }
    public string EsPropductType { get; set; }
    public string CaPropductType { get; set; }
}