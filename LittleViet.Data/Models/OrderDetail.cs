using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.Models;

    internal class OrderDetail : AuditableEntity
    {
        public Guid ServingId { get; set; }
        public Guid OrderId { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
    }

