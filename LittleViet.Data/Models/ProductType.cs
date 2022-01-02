using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.Models
{
    internal class ProductType: AuditableEntity
    {
        public string Name { get; set; }
        public string ESName { get; set; }
        public string CAName { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
    }
}
