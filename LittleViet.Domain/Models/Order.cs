﻿namespace LittleViet.Domain.Models;

public class Order : AuditableEntity
{
    public Guid AccountId { get; set; }
    public virtual Account Account { get; set; }
    public OrderType OrderType { get; set; }
    public double TotalPrice { get; set; }
    public PaymentType PaymentType { get; set; }
    public OrderStatus OrderStatus { get; set; } 
    public DateTime PickupTime { get; set; }
    public string LastStripeSessionId { get; set; }
    public virtual ICollection<OrderDetail> OrderDetails { get; set; }
}