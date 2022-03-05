using LittleViet.Data.Models;

namespace LittleViet.Data.ViewModels;

public class CreateServingViewModel
{
    public string Name { get; set; }
    public int NumberOfPeople { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public Guid ProductId { get; set; }
}

public class UpdateServingViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int NumberOfPeople { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public Guid ProductId { get; set; }
}

public class GenericServingViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int NumberOfPeople { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public Guid ProductId { get; set; }
}

public class ServingViewDetailsModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int NumberOfPeople { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public Guid ProductId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? CreatedDate { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid UpdatedBy { get; set; }
}

public class GetListServingParameters : BaseListQueryParameters<Serving>
{
    public string Name { get; set; }
    public int? NumberOfPeople { get; set; }
    public string Description { get; set; }
    public double? PriceFrom { get; set; }
    public double? PriceTo { get; set; }
    public Guid? ProductId { get; set; }
}