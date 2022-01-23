﻿namespace LittleViet.Data.ViewModels;

public class CreateServingViewModel
{
    public Guid CreatedBy { get; set; }
    public string Name { get; set; }
    public int NumberOfPeople { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public Guid ProductId { get; set; }
}

public class UpdateServingViewModel
{
    public Guid Id { get; set; }
    public Guid UpdatedBy { get; set; }
    public string Name { get; set; }
    public int NumberOfPeople { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public Guid ProductId { get; set; }
}

public class ServingViewModel
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