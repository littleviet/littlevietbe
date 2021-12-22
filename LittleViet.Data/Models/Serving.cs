using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleViet.Data.Models
{
    internal class Serving
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int NumberOfPeople { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public double Price { get; set; }
        public bool IsDeleted { get; set; }
    }
}
