﻿using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.Models;

public class Product : AuditableEntity
{
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public ProductStatus Status { get; set; }
    public double Price { get; set; }
    public Guid ProductTypeId { get; set; }

    public virtual ProductType ProductType { get; set; }
}

