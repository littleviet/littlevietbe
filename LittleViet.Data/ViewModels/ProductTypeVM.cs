namespace LittleViet.Data.ViewModels;

    public class CreateProductTypeVM
    {
        public Guid CreatedBy { get; set; }
        public string Name { get; set; }
        public string ESName { get; set; }
        public string CAName { get; set; }
        public string Description { get; set; }
    }

    public class UpdateProductTypeVM
    {
        public Guid UpdatedBy { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ESName { get; set; }
        public string CAName { get; set; }
        public string Description { get; set; }
    }

